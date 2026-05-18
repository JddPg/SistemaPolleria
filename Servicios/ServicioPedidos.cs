using SistemaPolleria.Modelos;
using SistemaPolleria.Repositorios;
using SistemaPolleria.Eventos;

namespace SistemaPolleria.Servicios;

public class ServicioPedidos : IServicio<Pedido>
{
    private readonly RepositorioPedidos _repositorio;
    private readonly RepositorioClientes _repoClientes;
    private readonly PublicadorEventos _publicador;

    public ServicioPedidos(
        RepositorioPedidos repositorio,
        RepositorioClientes repoClientes,
        PublicadorEventos publicador)
    {
        _repositorio = repositorio;
        _repoClientes = repoClientes;
        _publicador = publicador;
    }

    public void Crear(Pedido pedido)
    {
        if (pedido.Detalles.Count == 0)
            throw new ArgumentException("El pedido debe tener al menos un producto.");

        var cliente = _repoClientes.ObtenerPorId(pedido.ClienteId);
        if (cliente == null)
            throw new ArgumentException($"No existe un cliente con ID {pedido.ClienteId}.");

        pedido.NombreCliente = cliente.Nombre;
        _repositorio.Guardar(pedido);

      
        _publicador.NotificarPedidoCreado(pedido.Id, pedido.NombreCliente, pedido.Total, pedido.Fecha);

        Console.WriteLine($"\n Pedido #{pedido.Id} creado exitosamente. Total: ${pedido.Total:F0}");
    }

    public List<Pedido> ObtenerTodos() => _repositorio.ObtenerTodos();

    public Pedido? ObtenerPorId(int id) => _repositorio.ObtenerPorId(id);

    public void Actualizar(Pedido pedido)
    {
        _repositorio.Actualizar(pedido);
        Console.WriteLine($"\n Pedido actualizado exitosamente.");
    }

    public void Eliminar(int id)
    {
        var pedido = _repositorio.ObtenerPorId(id);
        if (pedido == null)
            throw new ArgumentException($"No existe un pedido con ID {id}.");

        _repositorio.Eliminar(id);
        Console.WriteLine($"\n Pedido eliminado exitosamente.");
    }

    public void CambiarEstado(int pedidoId, EstadoPedido nuevoEstado)
    {
        var pedido = _repositorio.ObtenerPorId(pedidoId);
        if (pedido == null)
            throw new ArgumentException($"No existe un pedido con ID {pedidoId}.");

        var estadoAnterior = pedido.Estado;
        pedido.Estado = nuevoEstado;
        _repositorio.Actualizar(pedido);

       
        _publicador.NotificarEstadoCambiado(pedidoId, estadoAnterior, nuevoEstado);
    }

   
    public List<Pedido> ObtenerPorEstado(EstadoPedido estado) =>
        _repositorio.ObtenerTodos()
            .Where(p => p.Estado == estado)
            .ToList();

    public decimal TotalVentasDelDia() =>
        _repositorio.ObtenerTodos()
            .Where(p => p.Fecha.Date == DateTime.Today && p.Estado != EstadoPedido.Cancelado)
            .Sum(p => p.Total);

    public List<ConsultaVentas> ProductosMasVendidos() =>
        _repositorio.ObtenerTodos()
            .Where(p => p.Estado != EstadoPedido.Cancelado)
            .SelectMany(p => p.Detalles)
            .GroupBy(d => d.NombreProducto)
            .Select(g => new ConsultaVentas(
                g.Key,
                g.Sum(d => d.Cantidad),
                g.Sum(d => d.Subtotal)
            ))
            .OrderByDescending(c => c.CantidadVendida)
            .ToList();
}