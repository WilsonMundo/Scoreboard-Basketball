using AngularApp1.Server;
using AngularApp1.Server.Application.Hubs;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Application.Mapping;
using AngularApp1.Server.Application.Services;
using AngularApp1.Server.Application.Services.Auth;
using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Infrastructure;
using AngularApp1.Server.Infrastructure.Repository;
using AngularApp1.Server.Infrastructure.Repository.Auth;
using AngularApp1.Server.Infrastructure.Utils;
using AngularApp1.Server.Middelware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(o =>
o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<GameProfile>();
});
var cadenaConexion = builder.Configuration["SQLConnection"]
    ?? throw new InvalidOperationException("Cadena de conexión vacía (SQLConnection).");

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta Jwt:Key en configuración.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddScoped<DatabaseConnectionManager>();
builder.Services.AddDbContext<RegisterDBContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<DatabaseConnectionManager>();
    var connectionString = connectionManager.ValidateConnectionString(cadenaConexion, "UserBiciLink");
    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<DatabaseConnectionManager>();
    var connectionString = connectionManager.ValidateConnectionString(cadenaConexion, "Scoreboard");
    options.UseSqlServer(connectionString);
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<RegisterDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidIssuer = jwtIssuer,

        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        ValidAudience = jwtAudience,

        ValidateLifetime = true,


        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Scoreboard API",
        Version = "v1"
    });
});

builder.Services.AddScoped<ResponseService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUtilsRepository,UtilsRepository>();


var app = builder.Build();
var enableSwagger = app.Configuration.GetValue<bool>("Swagger:Enabled");
var fwd = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor
                     | ForwardedHeaders.XForwardedProto
                     | ForwardedHeaders.XForwardedHost
};


// Con proxies delante, confíar en los encabezados
fwd.KnownNetworks.Clear();
fwd.KnownProxies.Clear();
app.UseForwardedHeaders(fwd);


if (enableSwagger || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.RoutePrefix = "docs"; // /docs en vez de /swagger
        o.SwaggerEndpoint("../swagger/v1/swagger.json", "Scoreboard API v1");
    });
}


app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<GameHub>("/hubs/game");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // aplica migraciones al levantar
}

app.Run();
try
{
   
    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}