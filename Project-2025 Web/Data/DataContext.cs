using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data.Entities;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Plan> Plans { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Plan>().ToTable("Plans");
        modelBuilder.Entity<Reservation>().ToTable("Reservations");
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Role>().ToTable("Roles");

        // Relación Plan - Reservation
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Plan)
            .WithMany()
            .HasForeignKey(r => r.Id_Plan)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación Role - User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Role - Permission (muchos a muchos)
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        // Seed de permisos
        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Gestionar Paquetes" },
            new Permission { Id = 2, Name = "Gestionar Usuarios" },
            new Permission { Id = 3, Name = "Gestionar Roles" },
            new Permission { Id = 4, Name = "Ver Reservas" },
            new Permission { Id = 5, Name = "Gestionar Reservas" },
            new Permission { Id = 6, Name = "Crear comentarios en el blog" },
            new Permission { Id = 7, Name = "Gestionar Blog" },
            new Permission { Id = 8, Name = "Hacer Reservas" }
        );

        // Seed de roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Usuario" }
        );

        // Seed de RolePermissions
        modelBuilder.Entity<RolePermission>().HasData(
            // Admin - todos los permisos
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },
            new RolePermission { RoleId = 1, PermissionId = 8 },


            // Usuario - solo permisos básicos
            new RolePermission { RoleId = 2, PermissionId = 4 },
            new RolePermission { RoleId = 2, PermissionId = 6 },
            new RolePermission { RoleId = 2, PermissionId = 8 }
        );
    }
}


