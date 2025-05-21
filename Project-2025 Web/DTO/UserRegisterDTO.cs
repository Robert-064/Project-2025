using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.DTOs
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int RoleId { get; set; }
    }
}
