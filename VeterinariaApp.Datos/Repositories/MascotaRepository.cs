using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Entidades;

namespace VeterinariaApp.Datos.Repositories;

public class MascotaRepository : IMascotaRepository
{
    private readonly VeterinariaContext _context;

    public MascotaRepository(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Mascota>> ObtenerTodosAsync(string? especie = null)
    {
        var query = _context.Mascotas.Include(m => m.Propietario).AsQueryable();

        if (!string.IsNullOrWhiteSpace(especie))
        {
            query = query.Where(m => m.Especie.Contains(especie));
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Mascota>> ObtenerPorPropietarioAsync(int propietarioId)
    {
        return await _context.Mascotas
            .Include(m => m.Propietario)
            .Where(m => m.PropietarioId == propietarioId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> ObtenerEspeciesAsync()
    {
        return await _context.Mascotas
            .Select(m => m.Especie)
            .Distinct()
            .OrderBy(e => e)
            .ToListAsync();
    }

    public async Task<Mascota?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mascotas
            .Include(m => m.Propietario)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> ExisteNombreParaPropietarioAsync(int propietarioId, string nombre, int? idAIgnorar = null)
    {
        return await _context.Mascotas.AnyAsync(m =>
            m.PropietarioId == propietarioId &&
            m.Nombre == nombre &&
            (idAIgnorar == null || m.Id != idAIgnorar));
    }

    public async Task<Mascota> CrearAsync(Mascota mascota)
    {
        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();
        return mascota;
    }

    public async Task ActualizarAsync(Mascota mascota)
    {
        _context.Mascotas.Update(mascota);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Mascota mascota)
    {
        _context.Mascotas.Remove(mascota);
        await _context.SaveChangesAsync();
    }
}
