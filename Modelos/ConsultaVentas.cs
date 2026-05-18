namespace SistemaPolleria.Modelos;

public record ConsultaVentas(
    string NombreProducto,
    int CantidadVendida,
    decimal TotalGenerado
);