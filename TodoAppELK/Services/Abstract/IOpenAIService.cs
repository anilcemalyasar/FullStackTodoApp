using TodoAppELK.Models.DTOs;
using TodoAppELK.Models.Domain;

namespace TodoAppELK.Services.Abstract
{
    public interface IOpenAIService
    {
        Task<string> AnalyzeTodoListAsync(List<Todo> todos);
        Task<string> GenerateMotivationalMessageAsync(int completedCount, int totalCount);
        Task<AiTodoSuggestionDto> GenerateSingleTodoSuggestionAsync(List<Todo> todos);
    }
}
