using SistemaPolleria.Modelos;

namespace SistemaPolleria.Eventos;

// EventArgs para cuando se crea un pedido
public class PedidoCreadoEventArgs : EventArgs
{
    public int PedidoId { get; }
    public string NombreCliente { get; }
    public decimal Total { get; }
    public DateTime Fecha { get; }

    public PedidoCreadoEventArgs(int pedidoId, string nombreCliente, decimal total, DateTime fecha)
    {
        PedidoId = pedidoId;
        NombreCliente = nombreCliente;
        Total = total;
        Fecha = fecha;
    }
}

// EventArgs para cuando cambia el estado de un pedido
public class EstadoCambiadoEventArgs : EventArgs
{
    public int PedidoId { get; }
    public EstadoPedido EstadoAnterior { get; }
    public EstadoPedido EstadoNuevo { get; }

    public EstadoCambiadoEventArgs(int pedidoId, EstadoPedido estadoAnterior, EstadoPedido estadoNuevo)
    {
        PedidoId = pedidoId;
        EstadoAnterior = estadoAnterior;
        EstadoNuevo = estadoNuevo;
    }
}

// Clase que publica los eventos 
public class PublicadorEventos
{
    // Evento 1: se dispara cuando se crea un pedido
    public event EventHandler<PedidoCreadoEventArgs>? PedidoCreado;

    // Evento 2: se dispara cuando cambia el estado de un pedido
    public event EventHandler<EstadoCambiadoEventArgs>? EstadoCambiado;

    public void NotificarPedidoCreado(int pedidoId, string nombreCliente, decimal total, DateTime fecha)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n EVENTO: Nuevo pedido #{pedidoId} creado para {nombreCliente} por ${total:F0}");
        Console.ResetColor();
        PedidoCreado?.Invoke(this, new PedidoCreadoEventArgs(pedidoId, nombreCliente, total, fecha));
    }

    public void NotificarEstadoCambiado(int pedidoId, EstadoPedido anterior, EstadoPedido nuevo)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n EVENTO: Pedido #{pedidoId} cambió de {anterior} → {nuevo}");
        Console.ResetColor();
        EstadoCambiado?.Invoke(this, new EstadoCambiadoEventArgs(pedidoId, anterior, nuevo));
    }
}