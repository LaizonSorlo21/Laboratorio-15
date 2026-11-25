using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Entidades;

namespace VeterinariaApp.Datos.Repositories;

public class PropietarioRepository : IPropietarioRepository
{
    private readonly VeterinariaContext _context;

    public PropietarioRepository(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Propietario>> ObtenerTodosAsync(string? nombre = null)
    {
        var query = _context.Propietarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(p => p.Nombre.Contains(nombre) || p.Apellido.Contains(nombre));
        }

        return await query
            .Include(p => p.Mascotas)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Propietario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Propietarios
            .Include(p => p.Mascotas)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Propietario?> ObtenerPorIdConMascotasAsync(int id)
    {
        return await _context.Propietarios
            .Include(p => p.Mascotas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _context.Propietarios.AnyAsync(p => p.Email == email);
    }

    public async Task<Propietario> CrearAsync(Propietario propietario)
    {
        _context.Propietarios.Add(propietario);
        await _context.SaveChangesAsync();
        return propietario;
    }

    public async Task ActualizarAsync(Propietario propietario)
    {
        _context.Propietarios.Update(propietario);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Propietario propietario)
    {
        _context.Propietarios.Remove(propietario);
        await _context.SaveChangesAsync();
    }
}
