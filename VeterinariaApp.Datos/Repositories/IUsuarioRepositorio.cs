using VeterinariaApp.Entidades;

namespace VeterinariaApp.Datos.Repositories;

public interface IUsuarioRepositorio
{
    Task<Usuario?> BuscarPorCorreoAsync(string correo);

    Task AgregarAsync(Usuario usuario);

    Task GuardarAsync();
}
