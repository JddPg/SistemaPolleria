using Castle.DynamicProxy;

namespace SistemaPolleria.Interceptores;

public static class FabricaServicios
{
    private static readonly ProxyGenerator _generador = new();

    public static T Crear<T>(T servicio) where T : class
    {
        var interceptores = new IInterceptor[]
        {
            new InterceptorLogging(),
            new InterceptorValidacion()
        };

        return _generador.CreateInterfaceProxyWithTarget(servicio, interceptores);
    }
}