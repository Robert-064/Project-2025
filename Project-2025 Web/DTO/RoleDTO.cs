﻿using Project_2025_Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50)]
        public string Name { get; set; }

        public List<int> SelectedPermissionIds { get; set; } = new();

        public List<Permission>? AvailablePermissions { get; set; }
    }

}
