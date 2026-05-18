using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using SistemaPolleria.Modelos;

namespace SistemaPolleria.Repositorios;

public class RepositorioPedidos : IRepositorio<Pedido>
{
    private readonly string _rutaPedidos = "Datos/pedidos.csv";
    private readonly string _rutaDetalles = "Datos/detalles_pedido.csv";

    public RepositorioPedidos()
    {
        if (!Directory.Exists("Datos"))
            Directory.CreateDirectory("Datos");
        if (!File.Exists(_rutaPedidos))
            File.WriteAllText(_rutaPedidos, "Id,ClienteId,NombreCliente,EmpleadoId,Fecha,Estado\n");
        if (!File.Exists(_rutaDetalles))
            File.WriteAllText(_rutaDetalles, "Id,PedidoId,ProductoId,NombreProducto,Cantidad,PrecioUnitario\n");
    }

    public List<Pedido> ObtenerTodos()
    {
        using var reader = new StreamReader(_rutaPedidos);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        var pedidos = csv.GetRecords<PedidoCsv>().ToList();
        var detalles = ObtenerDetalles();

        return pedidos.Select(p => new Pedido
        {
            Id = p.Id,
            ClienteId = p.ClienteId,
            NombreCliente = p.NombreCliente,
            EmpleadoId = p.EmpleadoId,
            Fecha = p.Fecha,
            Estado = Enum.Parse<EstadoPedido>(p.Estado),
            Detalles = detalles.Where(d => d.PedidoId == p.Id).ToList()
        }).ToList();
    }

    public Pedido? ObtenerPorId(int id) =>
        ObtenerTodos().FirstOrDefault(p => p.Id == id);

    public void Guardar(Pedido pedido)
    {
        var todos = ObtenerTodos();
        pedido.Id = todos.Count > 0 ? todos.Max(p => p.Id) + 1 : 1;

        var detalles = ObtenerDetalles();
        int nextDetalleId = detalles.Count > 0 ? detalles.Max(d => d.Id) + 1 : 1;
        foreach (var detalle in pedido.Detalles)
        {
            detalle.Id = nextDetalleId++;
            detalle.PedidoId = pedido.Id;
        }

        todos.Add(pedido);
        EscribirPedidos(todos);
        EscribirDetalles(detalles.Concat(pedido.Detalles).ToList());
    }

    public void Actualizar(Pedido pedido)
    {
        var todos = ObtenerTodos();
        var indice = todos.FindIndex(p => p.Id == pedido.Id);
        if (indice >= 0) todos[indice] = pedido;
        EscribirPedidos(todos);
    }

    public void Eliminar(int id)
    {
        var todos = ObtenerTodos().Where(p => p.Id != id).ToList();
        var detalles = ObtenerDetalles().Where(d => d.PedidoId != id).ToList();
        EscribirPedidos(todos);
        EscribirDetalles(detalles);
    }

    private List<DetallePedido> ObtenerDetalles()
    {
        using var reader = new StreamReader(_rutaDetalles);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        return csv.GetRecords<DetallePedido>().ToList();
    }

    private void EscribirPedidos(List<Pedido> lista)
    {
        var registros = lista.Select(p => new PedidoCsv
        {
            Id = p.Id,
            ClienteId = p.ClienteId,
            NombreCliente = p.NombreCliente,
            EmpleadoId = p.EmpleadoId,
            Fecha = p.Fecha,
            Estado = p.Estado.ToString()
        }).ToList();

        using var writer = new StreamWriter(_rutaPedidos);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        csv.WriteRecords(registros);
    }

    private void EscribirDetalles(List<DetallePedido> lista)
    {
        using var writer = new StreamWriter(_rutaDetalles);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        csv.WriteRecords(lista);
    }
}

public class PedidoCsv
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = "";
    public int EmpleadoId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "";
}