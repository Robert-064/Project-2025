using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2025_Web.Data.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El plan es obligatorio")]
        [ForeignKey("Plan")]
        public int Id_Plan { get; set; }
        public Plan Plan { get; set; } = null!;

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [ForeignKey("User")]
        public int Id_User { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(50, ErrorMessage = "El estado no puede tener más de 50 caracteres")]
        public string Status { get; set; } = null!;

        [Range(1, 1000, ErrorMessage = "El número de personas debe estar entre 1 y 1000")]
        public int Person_Number { get; set; }
    }
}

