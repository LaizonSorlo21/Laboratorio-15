using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Entidades.Dtos;

// DTO de entrada: lo que llega del cliente al crear un Propietario.
public class PropietarioCrearDto
{
    [Required]
    [StringLength(60, MinimumLength = 2)]
    public string Nombre { get; set; } = null!;

    [Required]
    [StringLength(60, MinimumLength = 2)]
    public string Apellido { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(150)]
    public string? Direccion { get; set; }
}
