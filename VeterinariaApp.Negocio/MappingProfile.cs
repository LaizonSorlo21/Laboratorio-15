using AutoMapper;
using VeterinariaApp.Entidades;
using VeterinariaApp.Entidades.Dtos;

namespace VeterinariaApp.Negocio;

// Único MappingProfile de la capa de Negocio con todos los mapeos (CreateMap) de la aplicación.
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Propietario -> PropietarioDto (lectura)
        // Por qué: es lo que la API devuelve al cliente; la entidad no debe salir de la API.
        // Configuración especial: ForMember CantidadMascotas, se calcula contando la colección
        // de navegación en vez de serializar todas las mascotas (evita respuestas pesadas y ciclos).
        CreateMap<Propietario, PropietarioDto>()
            .ForMember(dest => dest.CantidadMascotas, opt => opt.MapFrom(src => src.Mascotas.Count));

        // PropietarioCrearDto -> Propietario (entrada, creación)
        // Por qué: es lo que llega del cliente y se convierte en entidad para guardarla; no se devuelve.
        // Configuración especial: ninguna, los nombres de propiedad coinciden.
        CreateMap<PropietarioCrearDto, Propietario>();

        // PropietarioActualizarDto -> Propietario (entrada, actualización)
        // Por qué: igual que el de creación, pero se usa con Map(origen, destino) sobre una
        // entidad ya existente para no perder su Id ni su colección de Mascotas.
        // Configuración especial: ninguna.
        CreateMap<PropietarioActualizarDto, Propietario>();

        // Mascota -> MascotaDto (lectura)
        // Por qué: es lo que la API devuelve al cliente; la entidad no debe salir de la API.
        // Configuración especial: ForMember NombrePropietario con MapFrom, en vez de devolver el
        // objeto Propietario completo (y crear un ciclo JSON), se devuelve solo su nombre completo.
        CreateMap<Mascota, MascotaDto>()
            .ForMember(dest => dest.NombrePropietario,
                opt => opt.MapFrom(src => $"{src.Propietario.Nombre} {src.Propietario.Apellido}"));

        // MascotaCrearDto -> Mascota (entrada, creación)
        // Por qué: es lo que llega del cliente y se convierte en entidad para guardarla; no se devuelve.
        // Configuración especial: ninguna, los nombres coinciden (incluido PropietarioId).
        CreateMap<MascotaCrearDto, Mascota>();

        // MascotaActualizarDto -> Mascota (entrada, actualización)
        // Por qué: se usa con Map(origen, destino) para actualizar una Mascota existente sin
        // perder su Id ni su referencia de navegación ya cargada.
        // Configuración especial: ninguna.
        CreateMap<MascotaActualizarDto, Mascota>();
    }
}
