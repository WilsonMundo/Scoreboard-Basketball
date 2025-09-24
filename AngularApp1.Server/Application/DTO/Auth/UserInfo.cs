using System.ComponentModel.DataAnnotations;

namespace AngularApp1.Server.Application.DTO.Auth
{
    public class UserInfo
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Este campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Login { get; set; } = null!;
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Este campo {0} es requerido")]
        public string Password { get; set; } = null!;
        public void ClearInputFields()
        {
            Login = String.Empty;
            Password = String.Empty;
        }
    }
}
