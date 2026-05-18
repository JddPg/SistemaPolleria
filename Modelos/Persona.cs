namespace SistemaPolleria.Modelos;

public abstract class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Telefono { get; set; } = "";

    public abstract string ObtenerRol();

    public override string ToString() =>
        $"[{Id}] {Nombre} - Tel: {Telefono} ({ObtenerRol()})";
}

public class Cliente : Persona
{
    public string Direccion { get; set; } = "";
    public override string ObtenerRol() => "Cliente";
}

public class Empleado : Persona
{
    public string Cargo { get; set; } = "";
    public override string ObtenerRol() => $"Empleado - {Cargo}";
}