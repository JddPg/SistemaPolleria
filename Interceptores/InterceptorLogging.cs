using Castle.DynamicProxy;

namespace SistemaPolleria.Interceptores;

public class InterceptorLogging : IInterceptor
{
    private readonly string _rutaLog = "Datos/log.txt";

    public void Intercept(IInvocation invocation)
    {
        string mensaje = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] Ejecutando: {invocation.Method.Name}";
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($" LOG: {invocation.Method.Name}");
        Console.ResetColor();

        File.AppendAllText(_rutaLog, mensaje + "\n");


        invocation.Proceed();

        string fin = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] Completado: {invocation.Method.Name}";
        File.AppendAllText(_rutaLog, fin + "\n");
    }
}