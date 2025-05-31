using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.Data.Entities
{
    public class Permission
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del permiso es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre del permiso no puede tener más de 100 caracteres")]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}
