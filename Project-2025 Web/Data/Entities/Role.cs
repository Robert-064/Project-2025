using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.Data.Entities
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre del rol no puede tener más de 50 caracteres")]
        public string Name { get; set; } = null!;

        // Relación uno a muchos
        public ICollection<User>? Users { get; set; }
    }
}
