namespace VeterinariaApp.Negocio.Exceptions;

// Excepción propia para violaciones de reglas de negocio.
// El controlador la atrapa y la convierte en 409 Conflict.
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string message) : base(message)
    {
    }
}
