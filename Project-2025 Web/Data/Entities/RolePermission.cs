﻿
namespace Project_2025_Web.Data.Entities
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Role Role { get; set; }
        public int PermissionId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Permission Permission { get; set; }
    }

}
