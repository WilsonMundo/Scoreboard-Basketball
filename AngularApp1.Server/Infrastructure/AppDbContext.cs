using AngularApp1.Server.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Game> Games => Set<Game>();
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

        }
    }
}
