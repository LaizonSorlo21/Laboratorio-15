using VeterinariaApp.Entidades;

namespace VeterinariaApp.Datos.Repositories;

public interface IPropietarioRepository
{
    Task<IEnumerable<Propietario>> ObtenerTodosAsync(string? nombre = null);

    Task<Propietario?> ObtenerPorIdAsync(int id);

    Task<Propietario?> ObtenerPorIdConMascotasAsync(int id);

    Task<bool> ExisteEmailAsync(string email);

    Task<Propietario> CrearAsync(Propietario propietario);

    Task ActualizarAsync(Propietario propietario);

    Task EliminarAsync(Propietario propietario);
}
