using System.ComponentModel.DataAnnotations;
using Project_2025_Web.Data;

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

        [Range(1, 5, ErrorMessage = "La dificultad debe estar entre 1 y 5")]
        public int Type_Difficulty { get; set; }

        [Range(1, 100, ErrorMessage = "Debe haber al menos 1 persona y como máximo 100")]
        public int Max_Persons { get; set; }

        [Range(0, 100, ErrorMessage = "La distancia debe estar entre 0 y 100 km")]
        [Display(Name = "Distancia en kilómetros")]
        public double Distance { get; set; }

        public string? ImageUrl1 { get; set; }
        public string? ImageUrl2 { get; set; }
    }
}

