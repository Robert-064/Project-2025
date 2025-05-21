using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; }
    }
}
