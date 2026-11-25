using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaApp.Entidades.Dtos;
using VeterinariaApp.Negocio.Exceptions;
using VeterinariaApp.Negocio.Services;

namespace VeterinariaApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _authServicio;

    public AuthController(IAuthServicio authServicio)
    {
        _authServicio = authServicio;
    }

    // POST api/auth/registro  -> 201 / 400 / 409
    [AllowAnonymous]
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _authServicio.RegistrarAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new { mensaje = "Usuario registrado correctamente." });
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // POST api/auth/login  -> 200 / 400 / 401
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var token = await _authServicio.LoginAsync(dto);
        if (token is null)
        {
            // Mismo 401 si el correo no existe o la clave es incorrecta.
            return Unauthorized(new { mensaje = "Credenciales inválidas." });
        }

        return Ok(token);
    }

    // GET api/auth/perfil  -> 200 / 401. Los datos salen de los claims del token, no de la BD.
    [Authorize]
    [HttpGet("perfil")]
    public ActionResult<PerfilDto> Perfil()
    {
        var correo = User.FindFirst("email")?.Value;
        var rol = User.FindFirst("role")?.Value;

        return Ok(new PerfilDto { Correo = correo ?? string.Empty, Rol = rol ?? string.Empty });
    }
}
