using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data.Entities;

namespace Project_2025_Web.Data
{
    public class DataContext : DbContext
    {
        // Constructor con inyección de dependencias
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // DbSets para todas las entidades
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<OtherEntity> OtherEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de nombres de tabla
            modelBuilder.Entity<Plan>().ToTable("Plans");
            modelBuilder.Entity<Reservation>().ToTable("Reservations");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<OtherEntity>().ToTable("OtherEntities");

            // Configuración de relaciones
            ConfigurePlanReservationRelationship(modelBuilder);
            ConfigureUserRoleRelationship(modelBuilder);

            // Datos iniciales (seed data)
            SeedRoles(modelBuilder);
        }

        private void ConfigurePlanReservationRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Plan)
                .WithMany(p => p.Reservations) // Asume que Plan tiene una colección de Reservations
                .HasForeignKey(r => r.Id_Plan)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureUserRoleRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users) // Asume que Role tiene una colección de Users
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Administrator with full access" },
                new Role { Id = 2, Name = "User", Description = "Standard user with basic access" }
            );
        }
    }
}