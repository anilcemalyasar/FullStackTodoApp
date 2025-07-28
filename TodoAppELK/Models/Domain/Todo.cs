using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoAppELK.Models.Domain
{
    public class Todo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incrementing primary key
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Description { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        [Required] // not null 
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required] // not null
        public int UserId { get; set; }
        public User user { get; set; } = null!; // Navigation property to User
    }
}
