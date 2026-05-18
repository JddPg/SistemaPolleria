using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using SistemaPolleria.Modelos;

namespace SistemaPolleria.Repositorios;

public class RepositorioClientes : IRepositorio<Cliente>
{
    private readonly string _ruta = "Datos/clientes.csv";

    public RepositorioClientes()
    {
        if (!Directory.Exists("Datos"))
            Directory.CreateDirectory("Datos");
        if (!File.Exists(_ruta))
            File.WriteAllText(_ruta, "Id,Nombre,Telefono,Direccion\n");
    }

    public List<Cliente> ObtenerTodos()
    {
        using var reader = new StreamReader(_ruta);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        return csv.GetRecords<Cliente>().ToList();
    }

    public Cliente? ObtenerPorId(int id) =>
        ObtenerTodos().FirstOrDefault(c => c.Id == id);

    public void Guardar(Cliente cliente)
    {
        var todos = ObtenerTodos();
        cliente.Id = todos.Count > 0 ? todos.Max(c => c.Id) + 1 : 1;
        todos.Add(cliente);
        Escribir(todos);
    }

    public void Actualizar(Cliente cliente)
    {
        var todos = ObtenerTodos();
        var indice = todos.FindIndex(c => c.Id == cliente.Id);
        if (indice >= 0) todos[indice] = cliente;
        Escribir(todos);
    }

    public void Eliminar(int id)
    {
        var todos = ObtenerTodos().Where(c => c.Id != id).ToList();
        Escribir(todos);
    }

    private void Escribir(List<Cliente> lista)
    {
        using var writer = new StreamWriter(_ruta);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        csv.WriteRecords(lista);
    }
}