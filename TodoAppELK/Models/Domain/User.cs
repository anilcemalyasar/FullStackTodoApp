using System.ComponentModel.DataAnnotations;

namespace TodoAppELK.Models.Domain
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Todo> Todos { get; set; } = new List<Todo>();
    }
}