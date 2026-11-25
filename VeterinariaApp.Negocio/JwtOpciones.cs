namespace VeterinariaApp.Negocio;

// Se enlaza con la sección "Jwt" de la configuración (la Clave llega por user-secrets o variables de entorno).
public class JwtOpciones
{
    public const string Seccion = "Jwt";

    public string Clave { get; set; } = null!;

    public string Emisor { get; set; } = null!;

    public string Audiencia { get; set; } = null!;

    public int MinutosDeVida { get; set; } = 60;
}
