using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2025_Web.Data.Entities
{
	[Table("Users")]
	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "El nombre de usuario es obligatorio")]
		[StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres")]
		public string Username { get; set; } = null!;

		[Required(ErrorMessage = "El correo es obligatorio")]
		[EmailAddress(ErrorMessage = "Formato de correo inválido")]
		[StringLength(100, ErrorMessage = "El correo no puede tener más de 100 caracteres")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "La contraseña es obligatoria")]
		[StringLength(200, ErrorMessage = "La contraseña no puede tener más de 200 caracteres")]
		public string PasswordHash { get; set; } = null!;

		// Campos para recuperación de contraseña
		[StringLength(100)]
		public string? PasswordResetToken { get; set; }

		public DateTime? ResetTokenExpires { get; set; }

		// Relación con el rol
		[Required]
		public int RoleId { get; set; }

		[ForeignKey("RoleId")]
		public Role Role { get; set; } = null!;
	}
}
