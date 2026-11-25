using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Microservicio 2: catálogo de vacunas de la veterinaria.
// Datos en memoria (sin base de datos) y sin compartir clases con VeterinariaApp.Api.
var builder = WebApplication.CreateBuilder(args);

// Render / contenedores: escucha en el puerto que entrega la plataforma (variable PORT).
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Misma clave, emisor y audiencia que la API de la S14 (la clave llega por user-secrets o Jwt__Clave).
var clave = builder.Configuration["Jwt:Clave"];
if (string.IsNullOrWhiteSpace(clave) || clave.Length < 32)
{
    throw new InvalidOperationException("Jwt:Clave no está configurada o tiene menos de 32 caracteres.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Emisor"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "email",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Datos en memoria.
var vacunas = new List<Vacuna>
{
    new(1, "Antirrábica", "Perro", 12, "Obligatoria. Protege contra la rabia."),
    new(2, "Séxtuple (DHPPI+L)", "Perro", 12, "Moquillo, hepatitis, parvovirus, parainfluenza y leptospirosis."),
    new(3, "Bordetella (tos de las perreras)", "Perro", 12, "Recomendada para perros que van a guarderías."),
    new(4, "Triple felina", "Gato", 12, "Panleucopenia, calicivirus y rinotraqueítis."),
    new(5, "Leucemia felina", "Gato", 12, "Recomendada para gatos con acceso al exterior."),
    new(6, "Antirrábica felina", "Gato", 12, "Protege contra la rabia.")
};

// Todos los endpoints exigen autenticación (token JWT válido).
var grupo = app.MapGroup("/api/vacunas").RequireAuthorization();

// GET /api/vacunas?especie=perro  -> lista
grupo.MapGet("/", (string? especie) =>
    string.IsNullOrWhiteSpace(especie)
        ? Results.Ok(vacunas)
        : Results.Ok(vacunas.Where(v => v.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase))));

// GET /api/vacunas/{id}  -> por id (404 si no existe)
grupo.MapGet("/{id:int}", (int id) =>
    vacunas.FirstOrDefault(v => v.Id == id) is { } vacuna
        ? Results.Ok(vacuna)
        : Results.NotFound(new { mensaje = $"No existe una vacuna con id {id}." }));

// Salud del servicio (público, útil para la nube).
app.MapGet("/", () => Results.Ok(new { servicio = "VacunasApi", estado = "ok" }));

app.Run();

public record Vacuna(int Id, string Nombre, string Especie, int RefuerzoMeses, string Descripcion);
