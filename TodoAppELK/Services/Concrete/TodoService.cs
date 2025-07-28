using TodoAppELK.Models.Domain;
using TodoAppELK.Models.DTOs;
using TodoAppELK.Repositories.Abstract;
using TodoAppELK.Services.Abstract;

namespace TodoAppELK.Services.Concrete
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TodoResponseDto>> GetAllByUserIdAsync(int userId)
        {
            var todos = await _repository.GetAllByUserIdAsync(userId);
            return todos.Select(MapToResponseDto).ToList();
        }

        public async Task<TodoResponseDto?> GetByIdAsync(int id, int userId)
        {
            var todo = await _repository.GetByIdAsync(id, userId);
            return todo == null ? null : MapToResponseDto(todo);
        }

        public async Task<TodoResponseDto> AddAsync(AddTodoDto dto, int userId)
        {
            var todo = new Todo
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };
            var created = await _repository.AddAsync(todo);
            return MapToResponseDto(created);
        }

        public async Task<TodoResponseDto?> UpdateAsync(int id, UpdateTodoDto dto, int userId)
        {
            var updatedTodo = new Todo
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted
            };
            var todo = await _repository.UpdateAsync(id, updatedTodo, userId);
            return todo == null ? null : MapToResponseDto(todo);
        }

        public Task<bool> DeleteAsync(int id, int userId) => _repository.DeleteAsync(id, userId);

        private TodoResponseDto MapToResponseDto(Todo todo)
        {
            return new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title ?? "",
                Description = todo.Description ?? "",
                IsCompleted = todo.IsCompleted,
                CreatedDate = todo.CreatedDate
            };
        }
    }
}
