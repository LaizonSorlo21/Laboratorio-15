using System;

namespace VeterinariaApp.Entidades.Dtos;

// DTO de lectura: la entidad Mascota nunca sale de la API tal cual.
public class MascotaDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Especie { get; set; } = null!;

    public string? Raza { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public decimal Peso { get; set; }

    public int PropietarioId { get; set; }

    // ForMember + MapFrom: en vez de devolver el objeto Propietario completo,
    // se devuelve el nombre completo del propietario relacionado.
    public string NombrePropietario { get; set; } = null!;
}
