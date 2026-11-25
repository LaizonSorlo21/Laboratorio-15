namespace VeterinariaApp.Entidades.Dtos;

// DTO de lectura: lo que la API devuelve al cliente. Nunca se devuelve la entidad Propietario.
public class PropietarioDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    // Calculado en el ForMember del MappingProfile: evita devolver toda la colección de Mascotas.
    public int CantidadMascotas { get; set; }
}
