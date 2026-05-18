using SistemaPolleria.Modelos;
using SistemaPolleria.Repositorios;

namespace SistemaPolleria.Servicios;

public class ServicioProductos : IServicio<Producto>
{
    private readonly RepositorioProductos _repositorio;

    public ServicioProductos(RepositorioProductos repositorio)
    {
        _repositorio = repositorio;
    }

    public void Crear(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        if (producto.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.");

        _repositorio.Guardar(producto);
        Console.WriteLine($"\n Producto '{producto.Nombre}' registrado exitosamente.");
    }

    public List<Producto> ObtenerTodos() => _repositorio.ObtenerTodos();

    public Producto? ObtenerPorId(int id) => _repositorio.ObtenerPorId(id);

    public void Actualizar(Producto producto)
    {
        if (producto.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.");

        _repositorio.Actualizar(producto);
        Console.WriteLine($"\n Producto actualizado exitosamente.");
    }

    public void Eliminar(int id)
    {
        var producto = _repositorio.ObtenerPorId(id);
        if (producto == null)
            throw new ArgumentException($"No existe un producto con ID {id}.");

        _repositorio.Eliminar(id);
        Console.WriteLine($"\n Producto eliminado exitosamente.");
    }

 
    public List<Producto> ObtenerDisponibles() =>
        _repositorio.ObtenerTodos()
            .Where(p => p.Disponible)
            .OrderBy(p => p.Nombre)
            .ToList();

    public List<Producto> FiltrarPor(Func<Producto, bool> criterio) =>
        _repositorio.ObtenerTodos()
            .Where(criterio)
            .ToList();

    public decimal PrecioPromedio() =>
        _repositorio.ObtenerTodos() is { Count: > 0 } lista
            ? lista.Average(p => p.Precio)
            : 0;
}