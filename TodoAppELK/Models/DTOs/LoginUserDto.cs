using System.ComponentModel.DataAnnotations;

namespace TodoAppELK.Models.DTOs
{
    public class LoginUserDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        public string Password { get; set; } = string.Empty;
    }
}