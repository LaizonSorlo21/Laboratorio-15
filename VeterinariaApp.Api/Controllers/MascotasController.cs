using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaApp.Entidades.Dtos;
using VeterinariaApp.Negocio.Exceptions;
using VeterinariaApp.Negocio.Services;

namespace VeterinariaApp.Api.Controllers;

[ApiController]
[Authorize] // Por defecto todo exige token; se relaja con [AllowAnonymous] o se endurece con Roles.
[Route("api/[controller]")]
public class MascotasController : ControllerBase
{
    private readonly IMascotaService _mascotaService;

    public MascotasController(IMascotaService mascotaService)
    {
        _mascotaService = mascotaService;
    }

    // GET api/mascotas/especies  (PÚBLICO: cualquiera puede ver qué especies atiende la veterinaria)
    [AllowAnonymous]
    [HttpGet("especies")]
    public async Task<ActionResult<IEnumerable<string>>> ObtenerEspecies()
    {
        return Ok(await _mascotaService.ObtenerEspeciesAsync());
    }

    // GET api/mascotas?especie=perro  (cualquier usuario autenticado)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> ObtenerTodos([FromQuery] string? especie)
    {
        var mascotas = await _mascotaService.ObtenerTodosAsync(especie);
        return Ok(mascotas);
    }

    // GET api/mascotas/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MascotaDto>> ObtenerPorId(int id)
    {
        var mascota = await _mascotaService.ObtenerPorIdAsync(id);
        if (mascota is null)
        {
            return NotFound();
        }

        return Ok(mascota);
    }

    // POST api/mascotas  (Recepcionista o Veterinario)
    [Authorize(Roles = "Recepcionista,Veterinario")]
    [HttpPost]
    public async Task<ActionResult<MascotaDto>> Crear([FromBody] MascotaCrearDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var creada = await _mascotaService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { mensaje = $"No existe un propietario con id {dto.PropietarioId}." });
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // PUT api/mascotas/5  (Recepcionista o Veterinario)
    [Authorize(Roles = "Recepcionista,Veterinario")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] MascotaActualizarDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var actualizada = await _mascotaService.ActualizarAsync(id, dto);
            if (!actualizada)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { mensaje = $"No existe un propietario con id {dto.PropietarioId}." });
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // DELETE api/mascotas/5  (solo Veterinario)
    [Authorize(Roles = "Veterinario")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminada = await _mascotaService.EliminarAsync(id);
        if (!eliminada)
        {
            return NotFound();
        }

        return NoContent();
    }
}
