using VeterinariaApp.Entidades;

namespace VeterinariaApp.Datos.Repositories;

public interface IMascotaRepository
{
    Task<IEnumerable<Mascota>> ObtenerTodosAsync(string? especie = null);

    Task<IEnumerable<Mascota>> ObtenerPorPropietarioAsync(int propietarioId);

    Task<IEnumerable<string>> ObtenerEspeciesAsync();

    Task<Mascota?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteNombreParaPropietarioAsync(int propietarioId, string nombre, int? idAIgnorar = null);

    Task<Mascota> CrearAsync(Mascota mascota);

    Task ActualizarAsync(Mascota mascota);

    Task EliminarAsync(Mascota mascota);
}
