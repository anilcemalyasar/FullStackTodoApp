using System.ComponentModel.DataAnnotations;

namespace TodoAppELK.Models.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        public string Password { get; set; } = string.Empty;
    }
}