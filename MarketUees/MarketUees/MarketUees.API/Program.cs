using MarketUees.Application.Services;
using MarketUees.Domain.Interfaces;
using MarketUees.Domain.Interfaces.Repositories;
using MarketUees.Infrastructure.Identity;
using MarketUees.Infrastructure.Persistence;
using MarketUees.Infrastructure.Persistence.Repositories;
using MarketUees.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── MongoDB ──────────────────────────────────────────────────────────────────
var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"]!;
var mongoDatabaseName     = builder.Configuration["MongoDB:DatabaseName"]!;

var mongoClient   = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(mongoDatabase);
builder.Services.AddSingleton<MongoDbContext>();

// ── Identity sobre MongoDB (reemplaza AddEntityFrameworkStores) ──────────────
builder.Services
    .AddIdentity<AppIdentityUser, AppIdentityRole>()
    .AddMongoDbStores<AppIdentityUser, AppIdentityRole, Guid>(
        mongoConnectionString, mongoDatabaseName)
    .AddDefaultTokenProviders();

// ── Repositorios ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository,    UserRepository>();
builder.Services.AddScoped<IRoleRepository,    RoleRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICompraRepository,  CompraRepository>();
builder.Services.AddScoped<IResenaRepository,  ResenaRepository>();

// ── Servicios de aplicación ──────────────────────────────────────────────────
builder.Services.AddScoped<IJwtService,   JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<CompraService>();
builder.Services.AddScoped<ResenaService>();

// ── JWT ──────────────────────────────────────────────────────────────────────
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["JwtIssuer"],
            ValidAudience            = builder.Configuration["JwtAudience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]!))
        };
    });

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

// ── Seed: roles y admin inicial (igual al Parcial2) ──────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

    foreach (var role in new[] { "Admin", "User", "Vendedor" })
    {
        if (!await roleRepository.RoleExistsAsync(role))
            await roleRepository.CreateRole(role);
    }

    if (!await userRepository.UserExists("admin@marketuees.com"))
    {
        var admin = await userRepository.CreateUser(new MarketUees.Domain.Entities.Usuario
        {
            Email     = "admin@marketuees.com",
            Password  = "Admin123!",
            FirstName = "Admin",
            LastName  = "MarketUees"
        });
        await userRepository.AddToRoleSync(admin, "Admin");
    }
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
