namespace AngularApp1.Server.Domain.ModelRegister
{
    public class Rol
    {
        public short RolId { get; set; }

        public string Nombre { get; set; } = null!;

        public bool FlagEmpleado { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
