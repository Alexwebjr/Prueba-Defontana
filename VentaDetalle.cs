//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Prueba_Defontana
{
    using System;
    using System.Collections.Generic;
    
    public partial class VentaDetalle
    {
        public long ID_VentaDetalle { get; set; }
        public long ID_Venta { get; set; }
        public int Precio_Unitario { get; set; }
        public int Cantidad { get; set; }
        public int TotalLinea { get; set; }
        public long ID_Producto { get; set; }
    
        public virtual Producto Producto { get; set; }
        public virtual Venta Venta { get; set; }
    }
}
