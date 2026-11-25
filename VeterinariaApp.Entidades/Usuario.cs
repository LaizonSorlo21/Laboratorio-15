namespace VeterinariaApp.Entidades;

public partial class Usuario
{
    public int Id { get; set; }

    public string Correo { get; set; } = null!;

    // Hash BCrypt de la clave; nunca se guarda la clave en texto plano.
    public string ClaveHash { get; set; } = null!;

    public string Rol { get; set; } = null!;
}
