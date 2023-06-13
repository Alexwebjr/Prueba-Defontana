# Prueba-Defontana
Prueba Defontana – Desarrollador Backend .NET y SQL

# Consultas SQL
1. El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).

SELECT SUM(Total) AS Total_ventas
FROM Venta
WHERE Fecha >= DATEADD(DAY, -30, GETDATE())
--WHERE Venta.Fecha >= '2023-05-14'

2. El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).

SELECT TOP 1 ID_Venta, Total
FROM Venta
WHERE Fecha >= DATEADD(DAY, -30, GETDATE())
ORDER BY Total DESC;

3. Indicar cuál es el producto con mayor monto total de ventas.

SELECT TOP 1 p.ID_Producto, p.Nombre, SUM(vd.TotalLinea) AS MontoTotalVentas
FROM VentaDetalle vd
JOIN Producto p ON vd.ID_Producto = p.ID_Producto
JOIN Venta v ON vd.ID_Venta = v.ID_Venta
WHERE v.Fecha >= DATEADD(DAY, -30, GETDATE())
GROUP BY p.ID_Producto, p.Nombre
ORDER BY MontoTotalVentas DESC;

4. Indicar el local con mayor monto de ventas.

SELECT TOP 1 Venta.ID_Local, Local.Nombre, SUM(Venta.Total) AS MontoTotalVentas
FROM VentaDetalle
JOIN Venta ON VentaDetalle.ID_Venta = Venta.ID_Venta
JOIN Local ON Venta.ID_Local = Local.ID_Local
WHERE Venta.Fecha >= DATEADD(DAY, -30, GETDATE())
GROUP BY Venta.ID_Local, Local.Nombre
ORDER BY MontoTotalVentas DESC;

5. ¿Cuál es la marca con mayor margen de ganancias?

SELECT TOP 1 m.ID_Marca, m.Nombre, ((SUM(p.Precio_Venta) - SUM(p.Costo_Unitario)) / SUM(p.Costo_Unitario)) AS MargenGanancia
FROM Producto p
JOIN Marca m ON p.ID_Marca = m.ID_Marca
JOIN Venta ON VentaDetalle.ID_Venta = Venta.ID_Venta
WHERE Venta.Fecha >= DATEADD(DAY, -30, GETDATE())
GROUP BY m.ID_Marca, m.Nombre
ORDER BY MargenGanancia DESC;


6. ¿Cómo obtendrías cuál es el producto que más se vende en cada local?

SELECT ID_Local, ID_Producto, Nombre, CantidadVentas
FROM (
    SELECT V.ID_Local, VD.ID_Producto, P.Nombre, SUM(VD.Cantidad) AS CantidadVentas,
           ROW_NUMBER() OVER (PARTITION BY V.ID_Local ORDER BY COUNT(*) DESC) AS RN
    FROM VentaDetalle VD
	JOIN Venta V ON VD.ID_Venta = V.ID_Venta
    JOIN Producto P ON VD.ID_Producto = P.ID_Producto
	WHERE v.Fecha >= DATEADD(DAY, -30, GETDATE())
    GROUP BY V.ID_Local, VD.ID_Producto, P.Nombre
	--ORDER BY CantidadVentas DESC
) AS T
WHERE RN = 1;
