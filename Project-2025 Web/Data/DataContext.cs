using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Project_2025_Web.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales si las necesitas
            modelBuilder.Entity<Plan>().ToTable("Plans");
            modelBuilder.Entity<Reservation>().ToTable("Reservations");

            // Relación: Plan - Reservation (uno a muchos)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Plan)
                .WithMany() // si luego quieres tener una lista de reservas en Plan, aquí puedes poner .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.Id_Plan)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
