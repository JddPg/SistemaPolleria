namespace SistemaPolleria.Servicios;

public interface IServicio<T>
{
    void Crear(T entidad);
    List<T> ObtenerTodos();
    T? ObtenerPorId(int id);
    void Actualizar(T entidad);
    void Eliminar(int id);
}