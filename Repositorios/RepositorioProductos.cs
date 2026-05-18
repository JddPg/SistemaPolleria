using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using SistemaPolleria.Modelos;

namespace SistemaPolleria.Repositorios;

public class RepositorioProductos : IRepositorio<Producto>
{
    private readonly string _ruta = "Datos/productos.csv";

    public RepositorioProductos()
    {
        if (!Directory.Exists("Datos"))
            Directory.CreateDirectory("Datos");
        if (!File.Exists(_ruta))
            File.WriteAllText(_ruta, "Id,Nombre,Precio,Disponible,Tamano,Tipo\n");
    }

    public List<Producto> ObtenerTodos()
    {
        using var reader = new StreamReader(_ruta);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        var registros = csv.GetRecords<ProductoCsv>().ToList();
        return registros.Select<ProductoCsv, Producto>(r =>
        {
            if (r.Tipo == "Asado")
                return new ProductoAsado { Id = r.Id, Nombre = r.Nombre, Precio = r.Precio, Disponible = r.Disponible, Tamano = r.Tamano };
            else
                return new Acompanamiento { Id = r.Id, Nombre = r.Nombre, Precio = r.Precio, Disponible = r.Disponible };
        }).ToList();
    }

    public Producto? ObtenerPorId(int id) =>
        ObtenerTodos().FirstOrDefault(p => p.Id == id);

    public void Guardar(Producto producto)
    {
        var todos = ObtenerTodos();
        producto.Id = todos.Count > 0 ? todos.Max(p => p.Id) + 1 : 1;
        todos.Add(producto);
        Escribir(todos);
    }

    public void Actualizar(Producto producto)
    {
        var todos = ObtenerTodos();
        var indice = todos.FindIndex(p => p.Id == producto.Id);
        if (indice >= 0) todos[indice] = producto;
        Escribir(todos);
    }

    public void Eliminar(int id)
    {
        var todos = ObtenerTodos().Where(p => p.Id != id).ToList();
        Escribir(todos);
    }

    private void Escribir(List<Producto> lista)
    {
        var registros = lista.Select(p => new ProductoCsv
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Precio = p.Precio,
            Disponible = p.Disponible,
            Tamano = p is ProductoAsado pa ? pa.Tamano : "",
            Tipo = p is ProductoAsado ? "Asado" : "Acompanamiento"
        }).ToList();

        using var writer = new StreamWriter(_ruta);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        csv.WriteRecords(registros);
    }
}

public class ProductoCsv
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public bool Disponible { get; set; }
    public string Tamano { get; set; } = "";
    public string Tipo { get; set; } = "";
}