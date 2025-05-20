using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.DTOs
{
    public class ReservationDTO
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Plan")]
        public int Id_Plan { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Usuario")]
        public int Id_User { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Reservation")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener como máximo {1} caracteres")]
        [Display(Name = "Estado")]
        public string Status { get; set; } = null!;

        [Range(1, 1000, ErrorMessage = "El número de personas debe estar entre {1} y {2}")]
        [Display(Name = "# Personas")]
        public int Person_Number { get; set; }
    }
}


