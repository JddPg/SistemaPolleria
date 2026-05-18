namespace SistemaPolleria.Modelos;

public abstract class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public bool Disponible { get; set; } = true;

    public abstract string ObtenerTipo();

    public override string ToString() =>
        $"[{Id}] {Nombre} - ${Precio:F0} ({ObtenerTipo()})";
}

public class ProductoAsado : Producto
{
    public string Tamano { get; set; } = "";
    public override string ObtenerTipo() => $"Asado - {Tamano}";
}

public class Acompanamiento : Producto
{
    public override string ObtenerTipo() => "Acompañamiento";
}