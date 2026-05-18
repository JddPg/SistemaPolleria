namespace SistemaPolleria.Modelos;

public enum EstadoPedido
{
    Recibido,
    EnPreparacion,
    Listo,
    Entregado,
    Cancelado
}

public class DetallePedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = "";
    public int EmpleadoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public EstadoPedido Estado { get; set; } = EstadoPedido.Recibido;
    public List<DetallePedido> Detalles { get; set; } = new();
    public decimal Total => Detalles.Sum(d => d.Subtotal);

    public override string ToString() =>
        $"[{Id}] Cliente: {NombreCliente} | Estado: {Estado} | Total: ${Total:F0} | Fecha: {Fecha:dd/MM/yyyy}";
}