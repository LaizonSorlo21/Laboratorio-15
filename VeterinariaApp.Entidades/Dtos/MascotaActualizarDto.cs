using System;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Entidades.Dtos;

// DTO de entrada: lo que llega del cliente al actualizar una Mascota existente.
public class MascotaActualizarDto
{
    [Required]
    [StringLength(60, MinimumLength = 2)]
    public string Nombre { get; set; } = null!;

    [Required]
    [StringLength(40)]
    public string Especie { get; set; } = null!;

    [StringLength(40)]
    public string? Raza { get; set; }

    [Required]
    public DateTime FechaNacimiento { get; set; }

    [Range(0.1, 200)]
    public decimal Peso { get; set; }

    [Required]
    public int PropietarioId { get; set; }
}
