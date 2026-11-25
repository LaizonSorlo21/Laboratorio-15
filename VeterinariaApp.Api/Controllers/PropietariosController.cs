using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaApp.Entidades.Dtos;
using VeterinariaApp.Negocio.Exceptions;
using VeterinariaApp.Negocio.Services;

namespace VeterinariaApp.Api.Controllers;

[ApiController]
[Authorize] // Por defecto todo exige token; se endurece con Roles en las operaciones de escritura.
[Route("api/[controller]")]
public class PropietariosController : ControllerBase
{
    private readonly IPropietarioService _propietarioService;
    private readonly IMascotaService _mascotaService;

    public PropietariosController(IPropietarioService propietarioService, IMascotaService mascotaService)
    {
        _propietarioService = propietarioService;
        _mascotaService = mascotaService;
    }

    // GET api/propietarios?nombre=ana
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropietarioDto>>> ObtenerTodos([FromQuery] string? nombre)
    {
        var propietarios = await _propietarioService.ObtenerTodosAsync(nombre);
        return Ok(propietarios);
    }

    // GET api/propietarios/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropietarioDto>> ObtenerPorId(int id)
    {
        var propietario = await _propietarioService.ObtenerPorIdAsync(id);
        if (propietario is null)
        {
            return NotFound();
        }

        return Ok(propietario);
    }

    // GET api/propietarios/5/mascotas (ruta anidada)
    [HttpGet("{id:int}/mascotas")]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> ObtenerMascotasDelPropietario(int id)
    {
        var propietario = await _propietarioService.ObtenerPorIdAsync(id);
        if (propietario is null)
        {
            return NotFound();
        }

        var mascotas = await _mascotaService.ObtenerPorPropietarioAsync(id);
        return Ok(mascotas);
    }

    // POST api/propietarios  (Recepcionista o Veterinario)
    [Authorize(Roles = "Recepcionista,Veterinario")]
    [HttpPost]
    public async Task<ActionResult<PropietarioDto>> Crear([FromBody] PropietarioCrearDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var creado = await _propietarioService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // PUT api/propietarios/5  (Recepcionista o Veterinario)
    [Authorize(Roles = "Recepcionista,Veterinario")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] PropietarioActualizarDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var actualizado = await _propietarioService.ActualizarAsync(id, dto);
            if (!actualizado)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // DELETE api/propietarios/5  (solo Veterinario)
    [Authorize(Roles = "Veterinario")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var eliminado = await _propietarioService.EliminarAsync(id);
            if (!eliminado)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}
