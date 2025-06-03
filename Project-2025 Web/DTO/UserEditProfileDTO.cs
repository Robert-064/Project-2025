using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.DTO
{
    public class UserEditProfileDTO
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string RoleName { get; set; } = null!; // Solo mostrar, no editar

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string CurrentPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas nuevas no coinciden.")]
        public string? ConfirmNewPassword { get; set; }
    }

}
