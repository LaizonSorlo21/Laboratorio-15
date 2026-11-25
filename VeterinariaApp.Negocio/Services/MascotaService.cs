using AutoMapper;
using VeterinariaApp.Datos.Repositories;
using VeterinariaApp.Entidades;
using VeterinariaApp.Entidades.Dtos;
using VeterinariaApp.Negocio.Exceptions;

namespace VeterinariaApp.Negocio.Services;

public class MascotaService : IMascotaService
{
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IPropietarioRepository _propietarioRepository;
    private readonly IMapper _mapper;

    public MascotaService(
        IMascotaRepository mascotaRepository,
        IPropietarioRepository propietarioRepository,
        IMapper mapper)
    {
        _mascotaRepository = mascotaRepository;
        _propietarioRepository = propietarioRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MascotaDto>> ObtenerTodosAsync(string? especie = null)
    {
        var mascotas = await _mascotaRepository.ObtenerTodosAsync(especie);
        return _mapper.Map<IEnumerable<MascotaDto>>(mascotas);
    }

    public async Task<IEnumerable<string>> ObtenerEspeciesAsync()
    {
        return await _mascotaRepository.ObtenerEspeciesAsync();
    }

    public async Task<IEnumerable<MascotaDto>> ObtenerPorPropietarioAsync(int propietarioId)
    {
        var mascotas = await _mascotaRepository.ObtenerPorPropietarioAsync(propietarioId);
        return _mapper.Map<IEnumerable<MascotaDto>>(mascotas);
    }

    public async Task<MascotaDto?> ObtenerPorIdAsync(int id)
    {
        var mascota = await _mascotaRepository.ObtenerPorIdAsync(id);
        return mascota is null ? null : _mapper.Map<MascotaDto>(mascota);
    }

    public async Task<MascotaDto> CrearAsync(MascotaCrearDto dto)
    {
        var propietario = await _propietarioRepository.ObtenerPorIdAsync(dto.PropietarioId)
            ?? throw new KeyNotFoundException($"No existe un propietario con id {dto.PropietarioId}.");

        // Regla de negocio 3: un mismo propietario no puede tener dos mascotas con el mismo nombre.
        if (await _mascotaRepository.ExisteNombreParaPropietarioAsync(dto.PropietarioId, dto.Nombre))
        {
            throw new ReglaNegocioException(
                $"El propietario '{propietario.Nombre} {propietario.Apellido}' ya tiene una mascota llamada '{dto.Nombre}'.");
        }

        var mascota = _mapper.Map<Mascota>(dto);
        var creada = await _mascotaRepository.CrearAsync(mascota);

        creada.Propietario = propietario;
        return _mapper.Map<MascotaDto>(creada);
    }

    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var mascota = await _mascotaRepository.ObtenerPorIdAsync(id);
        if (mascota is null)
        {
            return false;
        }

        var propietario = await _propietarioRepository.ObtenerPorIdAsync(dto.PropietarioId)
            ?? throw new KeyNotFoundException($"No existe un propietario con id {dto.PropietarioId}.");

        if (await _mascotaRepository.ExisteNombreParaPropietarioAsync(dto.PropietarioId, dto.Nombre, id))
        {
            throw new ReglaNegocioException(
                $"El propietario '{propietario.Nombre} {propietario.Apellido}' ya tiene una mascota llamada '{dto.Nombre}'.");
        }

        _mapper.Map(dto, mascota);
        await _mascotaRepository.ActualizarAsync(mascota);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var mascota = await _mascotaRepository.ObtenerPorIdAsync(id);
        if (mascota is null)
        {
            return false;
        }

        await _mascotaRepository.EliminarAsync(mascota);
        return true;
    }
}
