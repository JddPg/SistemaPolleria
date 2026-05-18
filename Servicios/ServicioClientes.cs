using SistemaPolleria.Modelos;
using SistemaPolleria.Repositorios;

namespace SistemaPolleria.Servicios;

public class ServicioClientes : IServicio<Cliente>
{
    private readonly RepositorioClientes _repositorio;

    public ServicioClientes(RepositorioClientes repositorio)
    {
        _repositorio = repositorio;
    }

    public void Crear(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            throw new ArgumentException("El nombre del cliente no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(cliente.Telefono))
            throw new ArgumentException("El teléfono del cliente no puede estar vacío.");

        _repositorio.Guardar(cliente);
        Console.WriteLine($"\n Cliente '{cliente.Nombre}' registrado exitosamente.");
    }

    public List<Cliente> ObtenerTodos() => _repositorio.ObtenerTodos();

    public Cliente? ObtenerPorId(int id) => _repositorio.ObtenerPorId(id);

    public void Actualizar(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            throw new ArgumentException("El nombre del cliente no puede estar vacío.");

        _repositorio.Actualizar(cliente);
        Console.WriteLine($"\n Cliente actualizado exitosamente.");
    }

    public void Eliminar(int id)
    {
        var cliente = _repositorio.ObtenerPorId(id);
        if (cliente == null)
            throw new ArgumentException($"No existe un cliente con ID {id}.");

        _repositorio.Eliminar(id);
        Console.WriteLine($"\nCliente eliminado exitosamente.");
    }


    public List<Cliente> BuscarPorNombre(string nombre) =>
        _repositorio.ObtenerTodos()
            .Where(c => c.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public int TotalClientes() => _repositorio.ObtenerTodos().Count;
}