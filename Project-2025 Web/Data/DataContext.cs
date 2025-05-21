using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data.Entities;

namespace Project_2025_Web.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{
		}

		public DbSet<Plan> Plans { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Tablas personalizadas
			modelBuilder.Entity<Plan>().ToTable("Plans");
			modelBuilder.Entity<Reservation>().ToTable("Reservations");
			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<Role>().ToTable("Roles");

			// Relación: Plan → Reservation (1:N)
			modelBuilder.Entity<Reservation>()
				.HasOne(r => r.Plan)
				.WithMany()
				.HasForeignKey(r => r.Id_Plan)
				.OnDelete(DeleteBehavior.Cascade);

			// Relación: Role → User (1:N)
			modelBuilder.Entity<User>()
				.HasOne(u => u.Role)
				.WithMany()
				.HasForeignKey(u => u.RoleId)
				.OnDelete(DeleteBehavior.Restrict);

			// Datos semilla para Roles
			modelBuilder.Entity<Role>().HasData(
				new Role { Id = 1, Name = "Admin" },
				new Role { Id = 2, Name = "User" }
			);
		}
	}
}
