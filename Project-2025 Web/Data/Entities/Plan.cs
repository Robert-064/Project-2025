using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.Data.Entities
{
    public class Plan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string Description { get; set; } = null!;

        [Range(0, 999999, ErrorMessage = "El precio debe estar entre 0 y 999999")]
        [DataType(DataType.Currency)]
        public decimal Basic_Price { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de dificultad no puede tener más de 50 caracteres")]
        public string Type_Difficulty { get; set; } = null!;

        [Range(1, 1000, ErrorMessage = "Debe haber al menos 1 persona y como máximo 1000")]
        public int Max_Persons { get; set; }
    }
}

