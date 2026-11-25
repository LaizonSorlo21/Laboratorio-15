using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VeterinariaApp.Datos.Repositories;
using VeterinariaApp.Entidades;
using VeterinariaApp.Entidades.Dtos;
using VeterinariaApp.Negocio.Exceptions;

namespace VeterinariaApp.Negocio.Services;

public class AuthServicio : IAuthServicio
{
    // Rol que el servidor asigna a todo registro público (mínimo privilegio).
    // Los Veterinarios se crean desde la base de datos, nunca desde el cliente.
    public const string RolPorDefecto = "Recepcionista";

    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly JwtOpciones _jwt;

    public AuthServicio(IUsuarioRepositorio usuarioRepositorio, IOptions<JwtOpciones> jwt)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _jwt = jwt.Value;
    }

    public async Task RegistrarAsync(RegistroDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();

        if (await _usuarioRepositorio.BuscarPorCorreoAsync(correo) is not null)
        {
            throw new ReglaNegocioException($"Ya existe un usuario registrado con el correo '{correo}'.");
        }

        var usuario = new Usuario
        {
            Correo = correo,
            ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave),
            Rol = RolPorDefecto
        };

        await _usuarioRepositorio.AgregarAsync(usuario);
        await _usuarioRepositorio.GuardarAsync();
    }

    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepositorio.BuscarPorCorreoAsync(correo);

        // Mismo resultado (null) si el correo no existe o la clave es incorrecta: no revela cuál falló.
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash))
        {
            return null;
        }

        return GenerarToken(usuario);
    }

    private TokenDto GenerarToken(Usuario usuario)
    {
        // Claims mínimos: sub, email y role. Nunca la clave, el hash ni datos sensibles.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim("role", usuario.Rol)
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Clave));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddMinutes(_jwt.MinutosDeVida);

        var token = new JwtSecurityToken(
            issuer: _jwt.Emisor,
            audience: _jwt.Audiencia,
            claims: claims,
            expires: expiracion,
            signingCredentials: credenciales);

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracion = expiracion
        };
    }
}
