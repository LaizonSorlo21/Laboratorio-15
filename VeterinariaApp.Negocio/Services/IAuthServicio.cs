using VeterinariaApp.Entidades.Dtos;

namespace VeterinariaApp.Negocio.Services;

public interface IAuthServicio
{
    // Lanza ReglaNegocioException si el correo ya existe.
    Task RegistrarAsync(RegistroDto dto);

    // Devuelve null si las credenciales son inválidas (correo inexistente o clave incorrecta).
    Task<TokenDto?> LoginAsync(LoginDto dto);
}
