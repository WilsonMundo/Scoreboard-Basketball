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
            });
        }
    }
}
