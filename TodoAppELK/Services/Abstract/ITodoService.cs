using TodoAppELK.Models.DTOs;

namespace TodoAppELK.Services.Abstract
{
    public interface ITodoService
    {
        Task<List<TodoResponseDto>> GetAllByUserIdAsync(int userId);
        Task<TodoResponseDto?> GetByIdAsync(int id, int userId);
        Task<TodoResponseDto> AddAsync(AddTodoDto dto, int userId);
        Task<TodoResponseDto?> UpdateAsync(int id, UpdateTodoDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}