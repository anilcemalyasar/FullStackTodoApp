using TodoAppELK.Models.Domain;

namespace TodoAppELK.Services.Abstract
{
    public interface IOpenAIService
    {
        Task<string> GenerateTodoSuggestAsync(string userInput);
        Task<string> AnalyzeTodoListAsync(List<Todo> todos);
        Task<string> GenerateMotivationalMessageAsync(int completedCount, int totalCount);
    }
}
