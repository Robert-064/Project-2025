using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.DTOs
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}

