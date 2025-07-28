using TodoAppELK.Models.Domain;

namespace TodoAppELK.Repositories.Abstract
{
    public interface ITodoRepository
    {
        Task<List<Todo>> GetAllByUserIdAsync(int userId);
        Task<Todo?> GetByIdAsync(int id, int userId);
        Task<Todo> AddAsync(Todo todo);
        Task<Todo?> UpdateAsync(int id, Todo updatedTodo, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}