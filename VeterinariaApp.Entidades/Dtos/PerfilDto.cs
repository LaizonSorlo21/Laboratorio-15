namespace VeterinariaApp.Entidades.Dtos;

// DTO de salida de GET api/auth/perfil: datos leídos de los claims del token, no de la BD.
public class PerfilDto
{
    public string Correo { get; set; } = null!;

    public string Rol { get; set; } = null!;
}
