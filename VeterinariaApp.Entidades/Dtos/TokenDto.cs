namespace VeterinariaApp.Entidades.Dtos;

// DTO de salida del login: el JWT y su fecha de expiración (UTC).
public class TokenDto
{
    public string Token { get; set; } = null!;

    public DateTime Expiracion { get; set; }
}
