﻿using System.ComponentModel.DataAnnotations;

namespace Project_2025_Web.DTOs
{
    public class PlanDTO
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener como máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre del Plan")]
        public string Name { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "El campo {0} debe tener como máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Range(0, 999999, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Precio Básico")]
        public decimal Basic_Price { get; set; }

        [Range(1, 5, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Tipo de Dificultad")]
        public int Type_Difficulty { get; set; }


        [Range(1, 1000, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Máximo de Personas")]
        public int Max_Persons { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Distancia (Km)")]
        public double Distance { get; set; }

        [Display(Name = "Imagen 1")]
        [Required(ErrorMessage = "Debe subir una imagen principal")]
        public IFormFile ImageFile1 { get; set; } = null!; //Esto es para guardar imagenes

        [Display(Name = "Imagen 2")]
        [Required(ErrorMessage = "Debe subir una imagen secundaria")]
        public IFormFile ImageFile2 { get; set; } = null!;

        public string? ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; }
    }
}



