using VeterinariaApp.Entidades.Dtos;

namespace VeterinariaApp.Negocio.Services;

public interface IPropietarioService
{
    Task<IEnumerable<PropietarioDto>> ObtenerTodosAsync(string? nombre = null);

    Task<PropietarioDto?> ObtenerPorIdAsync(int id);

    Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto);

    Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto);

    Task<bool> EliminarAsync(int id);
}
