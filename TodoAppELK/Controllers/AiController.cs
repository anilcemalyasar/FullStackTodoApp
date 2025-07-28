using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAppELK.Data;
using TodoAppELK.Services.Abstract;

namespace TodoAppELK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly TodoDbContext _dbContext;
        private readonly IOpenAIService _openAIService;

        public AiController(TodoDbContext dbContext, IOpenAIService openAIService)
        {
            _dbContext = dbContext;
            _openAIService = openAIService;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet("suggest-todo")]
        public async Task<IActionResult> SuggestTodo()
        {
            int userId = GetUserIdFromToken();
            var todos = await _dbContext.Todos.Where(t => t.UserId == userId).ToListAsync();
            var suggestion = await _openAIService.GenerateSingleTodoSuggestionAsync(todos);
            return Ok(suggestion);
        }

        [HttpGet("analyze-todos")]
        public async Task<IActionResult> AnalyzeTodos()
        {
            int userId = GetUserIdFromToken();
            var todos = await _dbContext.Todos.Where(t => t.UserId == userId).ToListAsync();
            var analysis = await _openAIService.AnalyzeTodoListAsync(todos);
            return Ok(new { analysis });
        }

        [HttpGet("motivation")]
        public async Task<IActionResult> Motivation()
        {
            int userId = GetUserIdFromToken();
            var todos = await _dbContext.Todos.Where(t => t.UserId == userId).ToListAsync();
            var completedCount = todos.Count(t => t.IsCompleted);
            var totalCount = todos.Count;

            var message = await _openAIService.GenerateMotivationalMessageAsync(completedCount, totalCount);
            return Ok(new
            {
                message,
                completedCount,
                totalCount,
                completionRate = totalCount > 0 ? (double)completedCount / totalCount * 100 : 0
            });
        }
    }
}