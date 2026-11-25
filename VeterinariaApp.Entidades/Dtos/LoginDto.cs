using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Entidades.Dtos;

// DTO de entrada: credenciales para iniciar sesión.
public class LoginDto
{
    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Correo { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Clave { get; set; } = null!;
}
