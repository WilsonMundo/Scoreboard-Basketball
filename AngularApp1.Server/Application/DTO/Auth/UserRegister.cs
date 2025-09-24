using System.ComponentModel.DataAnnotations;

namespace AngularApp1.Server.Application.DTO.Auth
{
    public class UserRegister : UserInfo
    {

        [Required(ErrorMessage = "Este campo {0} es requerido")]
        public string confirmPassword { get; set; } = null!;
        [Required(ErrorMessage = "Este campo {0} es requerido")]
        public string name { get; set; } = null!;
        public string? direccion { get; set; }
        [Required(ErrorMessage = "Este campo {0}  es requerido")]
        public string nit { get; set; } = null!;
        [Required(ErrorMessage = "Este campo {0} es requerido")]
        public string telefono { get; set; } = null!;
    }
}
