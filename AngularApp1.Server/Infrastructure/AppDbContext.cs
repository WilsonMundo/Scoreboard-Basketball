using AngularApp1.Server.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Game> Games => Set<Game>();

        public DbSet<Team> Teams => Set<Team>(); 
        public DbSet<QuarterScore> QuarterScores => Set<QuarterScore>();



        public DbSet<TeamCatalog> TeamCatalogs => Set<TeamCatalog>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<GameTeam> GameTeams => Set<GameTeam>();
        public DbSet<GameRoster> GameRosters => Set<GameRoster>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Status).HasMaxLength(32).IsRequired();
                b.Property(x => x.Quarter).IsRequired();
                b.Property(x => x.QuarterSeconds).IsRequired();
                b.HasMany(x => x.Teams).WithOne(t => t.Game).HasForeignKey(t => t.GameId);
            });
            modelBuilder.Entity<Team>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Id).ValueGeneratedOnAdd();
                b.Property(t => t.Name).HasMaxLength(100).IsRequired();
                b.Property(t => t.IsHome).IsRequired();
                b.Property(t => t.Fouls);
                b.Property(t => t.LogoUrl).HasMaxLength(256);
                
            });
            modelBuilder.Entity<QuarterScore>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();

                b.HasIndex(x => new { x.GameId, x.TeamId, x.QuarterNumber }).IsUnique();

                b.Property(x => x.Points).IsRequired();

                // se mantiene cascada desde Game -> QuarterScores
                b.HasOne(x => x.Game)
                 .WithMany(g => g.QuarterScores)
                 .HasForeignKey(x => x.GameId)
                 .OnDelete(DeleteBehavior.Cascade);

                // se evita cascada desde Team -> QuarterScores para romper el ciclo */* error en migracion referencia circular **//
                b.HasOne(x => x.Team)
                 .WithMany()
                 .HasForeignKey(x => x.TeamId)
                 .OnDelete(DeleteBehavior.NoAction); // o .Restrict
            });
            modelBuilder.Entity<TeamCatalog>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();
                b.Property(x => x.Name).HasMaxLength(100).IsRequired();
                b.HasIndex(x => x.Name).IsUnique();
                b.Property(x => x.City).HasMaxLength(100);
                b.Property(x => x.LogoUrl).HasMaxLength(256);
            });


            modelBuilder.Entity<Player>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();
                b.Property(x => x.FullName).HasMaxLength(120).IsRequired();
                b.Property(x => x.Position).HasMaxLength(50);
                b.Property(x => x.Nationality).HasMaxLength(80);
                b.Property(x => x.HeightMeters).HasPrecision(4, 2); // ej. 2.05
            });


            modelBuilder.Entity<GameTeam>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();
                b.HasIndex(x => new { x.GameId, x.TeamId }).IsUnique();
                b.Property(x => x.IsHome).IsRequired();
                b.Property(x => x.Score).HasDefaultValue(0);
                b.Property(x => x.Fouls).HasDefaultValue(0);


                b.HasOne(x => x.Game)
                .WithMany() 
                .HasForeignKey(x => x.GameId)
                .OnDelete(DeleteBehavior.Cascade);


                b.HasOne(x => x.Team)
                .WithMany()
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<GameRoster>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();
                b.HasIndex(x => new { x.GameId, x.TeamId, x.PlayerId }).IsUnique();


                b.HasOne(x => x.Game)
                .WithMany() // navegación en Game opcional por ahora
                .HasForeignKey(x => x.GameId)
                .OnDelete(DeleteBehavior.Cascade);


                b.HasOne(x => x.Team)
                .WithMany()
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);


                b.HasOne(x => x.Player)
                .WithMany()
                .HasForeignKey(x => x.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
