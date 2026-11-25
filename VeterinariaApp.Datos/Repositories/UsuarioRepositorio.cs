using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Entidades;

namespace VeterinariaApp.Datos.Repositories;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly VeterinariaContext _context;

    public UsuarioRepositorio(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> BuscarPorCorreoAsync(string correo)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Correo == correo);
    }

    public async Task AgregarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public async Task GuardarAsync()
    {
        await _context.SaveChangesAsync();
    }
}
