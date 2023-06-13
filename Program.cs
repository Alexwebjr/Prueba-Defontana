using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba_Defontana
{
    class Program
    {
        static void Main(string[] args)
        {
            //Calling DbContext
            var db = new PruebaEntities();
            var datos = db.VentaDetalle
                .Include(v => v.Venta)
                .Include(v => v.Venta.Local)
                .Include(v => v.Producto)
                .Include(v => v.Producto.Marca)
                .ToList();

            //1. Consultar ventas de los últimos 30 días
            int from = -30; //dias a restar
            var fechaDesde = (DateTime.Now.Date).AddDays(from).Date;
            var ventas = datos
                .Where(v => v.Venta.Fecha >= fechaDesde).ToList()
                .OrderByDescending(v => v.Venta.Fecha)
                .ToList();

            //2. IMPRIMIR DETALLES:
            Console.WriteLine("2. Detalles de Venta:");
            //2.1 El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).

            var ventasTotales = ventas
                .GroupBy(vd => vd.Venta.ID_Venta)
                .Select(g => new
                {
                    VentaID = g.Key,
                    MontoTotal = g.Sum(vd => vd.TotalLinea),
                    CantidadTotal = g.Count()
                })
                .ToList();

            var montoTotalVentas = ventasTotales.Sum(v => v.MontoTotal);
            var cantidadTotalVentas = ventasTotales.Sum(v => v.CantidadTotal);

            Console.WriteLine();
            Console.WriteLine("1- Monto Total: " + montoTotalVentas + " | Cantidad total de ventas: " + cantidadTotalVentas);


            //2.2 El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).
            var ventaMaxima = ventas.OrderByDescending(x => x.Venta.Total).FirstOrDefault();
            Console.WriteLine();
            Console.WriteLine("2- Venta monto más alto: Fecha: " + ventaMaxima.Venta.Fecha + " | Total:" + ventaMaxima.Venta.Total);


            //2.3 Indicar cuál es el producto con mayor monto total de ventas.
            var productoConMayorMonto = ventas
                .GroupBy(vd => new { vd.ID_Producto, vd.Producto.Nombre })
                .Select(g => new { ProductoID = g.Key, MontoTotal = g.Sum(vd => vd.TotalLinea), Nombre = g.Key.Nombre })
                .OrderByDescending(p => p.MontoTotal)
                .FirstOrDefault();

            Console.WriteLine();
            Console.WriteLine("3- Producto mayor monto total de ventas: " + productoConMayorMonto.Nombre);


            //2.4 Indicar el Local con mayor monto de ventas.
            var local = ventas
                .GroupBy(v => new { v.Venta.ID_Local, v.Venta.Local.Nombre })
                .Select(g => new
                {
                    LocalID = g.Key,
                    MontoTotal = g.Sum(v => v.Venta.Total),
                    NombreLocal = g.Key.Nombre
                })
                .OrderByDescending(g => g.MontoTotal)
                .FirstOrDefault();

            Console.WriteLine();
            Console.WriteLine("4- Local con mayor monto de ventas: " + local.NombreLocal + " | " + local.MontoTotal);

            //2.5 ¿Cuál es la marca con mayor margen de ganancias?
            var marcasConMargenGanancias = ventas
                .GroupBy(vd => new { vd.Producto.ID_Marca, vd.Producto.Marca.Nombre })
                .Select(g => new
                {
                    ID_Marca = g.Key.ID_Marca,
                    Marca = g.Key.Nombre,
                    MargenGanancia = g.Sum(vd => (vd.Precio_Unitario - vd.Producto.Costo_Unitario) * vd.Cantidad)
                                    / g.Sum(vd => vd.Producto.Costo_Unitario * vd.Cantidad)
                })
                .OrderByDescending(m => m.MargenGanancia).FirstOrDefault();

            Console.WriteLine();
            Console.WriteLine("5- Marca con mayor margen de ganacias: Marca: " + marcasConMargenGanancias.Marca + " | Margen de Ganacia: " + marcasConMargenGanancias.MargenGanancia);

            //2.6 ¿Cómo obtendrías cuál es el producto que más se vende en cada local?
            var productosMasVendidosPorLocal = (from vd in ventas
                                                group vd by new { vd.Venta.ID_Local, vd.Producto.Nombre } into totals
                                                select new {
                                                    ID_Local = totals.Key.ID_Local,
                                                    Producto = totals.Key.Nombre,
                                                    CantidadVentas = totals.Sum(x => x.Cantidad)
                                                })
                                               .GroupBy(g => g.ID_Local)
                                               .Select(g => g.OrderByDescending(p => p.CantidadVentas).FirstOrDefault())
                                               .ToList();
            Console.WriteLine();
            Console.WriteLine("6- Producto que más se vende en cada local: ");
            foreach (var productoMasVendido in productosMasVendidosPorLocal)
            {
                Console.WriteLine("Local: " + productoMasVendido.ID_Local + " | Producto: " + productoMasVendido.Producto + " | Cantidad Vendida: " + productoMasVendido.CantidadVentas);
            }

            Console.WriteLine();
            Console.WriteLine("Presione alguna tecla para finalizar!");
            Console.ReadKey();

        }
    }
}
