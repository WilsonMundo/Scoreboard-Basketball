using AngularApp1.Server.Middelware;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AngularApp1.Server.Infrastructure
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connString = config.GetConnectionString("Default");
            var cadenaConexion = config["SQLConnection"]
               ?? throw new InvalidOperationException("Cadena de conexión vacía (SQLConnection).");

            var fakeAccessor = new HttpContextAccessor();

            var connectionManager = new DatabaseConnectionManager(fakeAccessor);
            var connectionString = connectionManager.ValidateConnectionString(cadenaConexion, "Scoreboard");

            // 4) Construir las opciones del DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new AppDbContext(options);
        }
    }
}

