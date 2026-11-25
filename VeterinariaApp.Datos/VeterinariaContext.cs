using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Entidades;

namespace VeterinariaApp.Datos;

public partial class VeterinariaContext : DbContext
{
    public VeterinariaContext(DbContextOptions<VeterinariaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Propietario> Propietarios { get; set; } = null!;

    public virtual DbSet<Mascota> Mascotas { get; set; } = null!;

    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Correo).HasMaxLength(120).IsRequired();
            entity.HasIndex(e => e.Correo).IsUnique();
            entity.Property(e => e.ClaveHash).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Rol).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<Propietario>(entity =>
        {
            entity.ToTable("Propietarios");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nombre).HasMaxLength(60).IsRequired();
            entity.Property(e => e.Apellido).HasMaxLength(60).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(120).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Direccion).HasMaxLength(150);
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.ToTable("Mascotas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nombre).HasMaxLength(60).IsRequired();
            entity.Property(e => e.Especie).HasMaxLength(40).IsRequired();
            entity.Property(e => e.Raza).HasMaxLength(40);
            entity.Property(e => e.Peso).HasColumnType("decimal(6,2)");

            entity.HasOne(d => d.Propietario)
                .WithMany(p => p.Mascotas)
                .HasForeignKey(d => d.PropietarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
