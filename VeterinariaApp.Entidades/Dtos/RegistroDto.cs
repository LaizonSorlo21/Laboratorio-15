using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Entidades.Dtos;

// DTO de entrada: registro de usuario. NO incluye el rol: lo decide siempre el servidor.
public class RegistroDto
{
    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Correo { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Clave { get; set; } = null!;
}
