using AngularApp1.Server.Domain.ModelRegister;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Infrastructure
{
    public  partial class RegisterDBContext : DbContext
    {
        public RegisterDBContext(DbContextOptions<RegisterDBContext> options)
            : base(options)
        {

        }
        public virtual DbSet<Rol> Rols { get; set; }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Rol");

                entity.Property(e => e.RolId).ValueGeneratedNever();
                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.ToTable("Usuario");

                entity.Property(e => e.IdUser).HasColumnName("idUser");
                entity.Property(e => e.Direccion)
                    .HasMaxLength(250)
                    .IsUnicode(false);
                entity.Property(e => e.Email)
                    .HasMaxLength(8000)
                    .IsUnicode(false)
                    .UseCollation("Latin1_General_100_BIN2")
                    .HasColumnName("email");
                entity.Property(e => e.FlagActivo).HasDefaultValue(true);
                entity.Property(e => e.Name)
                    .HasMaxLength(8000)
                    .IsUnicode(false)
                    .UseCollation("Latin1_General_100_BIN2")
                    .HasColumnName("name");
                entity.Property(e => e.Nit)
                    .HasMaxLength(8000)
                    .IsUnicode(false)
                    .UseCollation("Latin1_General_100_BIN2")
                    .HasColumnName("nit");
                entity.Property(e => e.Password)
                    .IsUnicode(false)
                    .HasColumnName("password");
                entity.Property(e => e.Telefono)
                    .HasMaxLength(8000)
                    .IsUnicode(false)
                    .UseCollation("Latin1_General_100_BIN2");

                entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.RolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Rol");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);



    }
}
