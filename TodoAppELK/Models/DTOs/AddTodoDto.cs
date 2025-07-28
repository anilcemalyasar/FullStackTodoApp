using System.ComponentModel.DataAnnotations;

namespace TodoAppELK.Models.DTOs
{
    public class AddTodoDto
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
    }
}