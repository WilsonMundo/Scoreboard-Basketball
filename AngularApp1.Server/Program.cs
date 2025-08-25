using AngularApp1.Server.Application.Hubs;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Application.Mapping;
using AngularApp1.Server.Application.Services;
using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Infrastructure;
using AngularApp1.Server.Infrastructure.Repository;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(o =>
o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<GameProfile>();
});

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();
var enableSwagger = app.Configuration.GetValue<bool>("Swagger:Enabled");
var fwd = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor
                     | ForwardedHeaders.XForwardedProto
                     | ForwardedHeaders.XForwardedHost
};


// Con proxies delante, confía en los encabezados
fwd.KnownNetworks.Clear();
fwd.KnownProxies.Clear();
app.UseForwardedHeaders(fwd);


if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.RoutePrefix = "docs"; // /docs en vez de /swagger
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "Scoreboard API v1");
    });
}


app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<GameHub>("/hubs/game");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // aplica migraciones al levantar
}

app.Run();
