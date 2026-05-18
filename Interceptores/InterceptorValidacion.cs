using Castle.DynamicProxy;

namespace SistemaPolleria.Interceptores;

public class InterceptorValidacion : IInterceptor
{
    private readonly string _rutaLog = "Datos/log.txt";

    public void Intercept(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
        }
        catch (ArgumentException ex)
        {
            string error = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ERROR VALIDACION en {invocation.Method.Name}: {ex.Message}";
            File.AppendAllText(_rutaLog, error + "\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n Error de validación: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            string error = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ERROR en {invocation.Method.Name}: {ex.Message}";
            File.AppendAllText(_rutaLog, error + "\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n Error inesperado: {ex.Message}");
            Console.ResetColor();
        }
    }
}