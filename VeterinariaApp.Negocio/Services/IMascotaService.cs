using VeterinariaApp.Entidades.Dtos;

namespace VeterinariaApp.Negocio.Services;

public interface IMascotaService
{
    Task<IEnumerable<MascotaDto>> ObtenerTodosAsync(string? especie = null);

    Task<IEnumerable<MascotaDto>> ObtenerPorPropietarioAsync(int propietarioId);

    Task<IEnumerable<string>> ObtenerEspeciesAsync();

    Task<MascotaDto?> ObtenerPorIdAsync(int id);

    Task<MascotaDto> CrearAsync(MascotaCrearDto dto);

    Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto);

    Task<bool> EliminarAsync(int id);
}
