using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre completo")]
        public string FullName { get; set; }
    }
}
