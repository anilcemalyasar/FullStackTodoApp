using Microsoft.EntityFrameworkCore;
using TodoAppELK.Data;
using TodoAppELK.Models.Domain;
using TodoAppELK.Repositories.Abstract;

namespace TodoAppELK.Repositories.Concrete
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _dbContext;

        // Dependency injection ile TodoDbContext'i alıyoruz.
        public TodoRepository(TodoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Todo>> GetAllByUserIdAsync(int userId)
        {
            return await _dbContext.Todos.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<Todo?> GetByIdAsync(int id, int userId)
        {
            return await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<Todo> AddAsync(Todo todo)
        {
            _dbContext.Todos.Add(todo);
            await _dbContext.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo?> UpdateAsync(int id, Todo updatedTodo, int userId)
        {
            var todo = await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (todo == null) return null;

            todo.Title = updatedTodo.Title;
            todo.Description = updatedTodo.Description;
            todo.IsCompleted = updatedTodo.IsCompleted;
            await _dbContext.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var todo = await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (todo == null) return false;
            _dbContext.Todos.Remove(todo);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
