using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using VeterinariaApp.Datos;
using VeterinariaApp.Datos.Repositories;
using VeterinariaApp.Negocio;
using VeterinariaApp.Negocio.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con esquema Bearer: el botón "Authorize" envía el header Authorization: Bearer <token>.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega solo el token JWT (sin la palabra Bearer)."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// DbContext (registra el DbContext; la cadena de conexión viene de user-secrets o variables de entorno).
// S15: en local se usa SQL Server; en la nube (Supabase) PostgreSQL. Se elige con Database:Provider.
var proveedorBd = builder.Configuration["Database:Provider"] ?? "SqlServer";
var cadena = builder.Configuration.GetConnectionString("VeterinariaConnection");
if (proveedorBd.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
{
    // Mantiene el comportamiento de DateTime de SQL Server (timestamp sin zona horaria).
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    builder.Services.AddDbContext<VeterinariaContext>(options => options.UseNpgsql(cadena));
}
else
{
    builder.Services.AddDbContext<VeterinariaContext>(options => options.UseSqlServer(cadena));
}

// Render / contenedores: escucha en el puerto que entrega la plataforma (variable PORT).
var puertoNube = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(puertoNube))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{puertoNube}");
}

// Repositorios (capa Datos).
builder.Services.AddScoped<IPropietarioRepository, PropietarioRepository>();
builder.Services.AddScoped<IMascotaRepository, MascotaRepository>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Servicios (capa Negocio).
builder.Services.AddScoped<IPropietarioService, PropietarioService>();
builder.Services.AddScoped<IMascotaService, MascotaService>();
builder.Services.AddScoped<IAuthServicio, AuthServicio>();

// AutoMapper con el único MappingProfile de la capa de Negocio.
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

// JWT: la clave NO está en el repositorio (user-secrets / variable de entorno Jwt__Clave).
builder.Services.Configure<JwtOpciones>(builder.Configuration.GetSection(JwtOpciones.Seccion));
var jwt = builder.Configuration.GetSection(JwtOpciones.Seccion).Get<JwtOpciones>()
          ?? throw new InvalidOperationException("Falta la sección 'Jwt' en la configuración.");

if (string.IsNullOrWhiteSpace(jwt.Clave) || jwt.Clave.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Clave no está configurada o tiene menos de 32 caracteres. " +
        "Configúrala con: dotnet user-secrets set \"Jwt:Clave\" \"<clave de 32+ caracteres>\" --project VeterinariaApp.Api");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Mantiene los nombres originales de los claims (sub, email, role) sin remapearlos.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emisor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "email",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline (Swagger muestra solo los DTOs, nunca las entidades).
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "VeterinariaApp API v1");
});

app.UseHttpsRedirection();

// Orden obligatorio: primero se autentica (quién eres) y luego se autoriza (qué puedes hacer).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
