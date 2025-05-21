using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string? Description { get; set; }

        // Propiedad de navegación inversa (opcional)
        public ICollection<User> User { get; set; } = new List<User>();
    }
}
    

