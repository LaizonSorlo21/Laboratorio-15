using AutoMapper;
using VeterinariaApp.Datos.Repositories;
using VeterinariaApp.Entidades;
using VeterinariaApp.Entidades.Dtos;
using VeterinariaApp.Negocio.Exceptions;

namespace VeterinariaApp.Negocio.Services;

public class PropietarioService : IPropietarioService
{
    private readonly IPropietarioRepository _propietarioRepository;
    private readonly IMapper _mapper;

    public PropietarioService(IPropietarioRepository propietarioRepository, IMapper mapper)
    {
        _propietarioRepository = propietarioRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PropietarioDto>> ObtenerTodosAsync(string? nombre = null)
    {
        var propietarios = await _propietarioRepository.ObtenerTodosAsync(nombre);
        return _mapper.Map<IEnumerable<PropietarioDto>>(propietarios);
    }

    public async Task<PropietarioDto?> ObtenerPorIdAsync(int id)
    {
        var propietario = await _propietarioRepository.ObtenerPorIdAsync(id);
        return propietario is null ? null : _mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto)
    {
        // Regla de negocio 1: no se puede registrar dos propietarios con el mismo correo.
        if (await _propietarioRepository.ExisteEmailAsync(dto.Email))
        {
            throw new ReglaNegocioException($"Ya existe un propietario registrado con el correo '{dto.Email}'.");
        }

        var propietario = _mapper.Map<Propietario>(dto);
        var creado = await _propietarioRepository.CrearAsync(propietario);
        return _mapper.Map<PropietarioDto>(creado);
    }

    public async Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto)
    {
        var propietario = await _propietarioRepository.ObtenerPorIdConMascotasAsync(id);
        if (propietario is null)
        {
            return false;
        }

        if (await _propietarioRepository.ExisteEmailAsync(dto.Email) && propietario.Email != dto.Email)
        {
            throw new ReglaNegocioException($"Ya existe un propietario registrado con el correo '{dto.Email}'.");
        }

        _mapper.Map(dto, propietario);
        await _propietarioRepository.ActualizarAsync(propietario);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var propietario = await _propietarioRepository.ObtenerPorIdConMascotasAsync(id);
        if (propietario is null)
        {
            return false;
        }

        // Regla de negocio 2: no se puede eliminar un propietario que todavía tiene mascotas registradas.
        if (propietario.Mascotas.Count > 0)
        {
            throw new ReglaNegocioException(
                $"No se puede eliminar al propietario '{propietario.Nombre} {propietario.Apellido}' " +
                "porque tiene mascotas registradas. Elimínalas primero.");
        }

        await _propietarioRepository.EliminarAsync(propietario);
        return true;
    }
}
