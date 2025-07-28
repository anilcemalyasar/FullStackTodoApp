using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TodoAppELK.Models.DTOs;
using TodoAppELK.Services.Abstract;

namespace TodoAppELK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _service;

        public TodoController(ITodoService service)
        {
            _service = service;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = GetUserIdFromToken();
            try
            {
                var todos = await _service.GetAllByUserIdAsync(userId);
                Log.Information("User {UserId} fetched all todos. Count: {Count}", userId, todos.Count);
                return Ok(todos);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching todos for user {UserId}", userId);
                return StatusCode(500, "An error occurred while fetching todos.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = GetUserIdFromToken();
            try
            {
                var todo = await _service.GetByIdAsync(id, userId);
                if (todo == null)
                {
                    Log.Warning("Todo with id {TodoId} not found for user {UserId}", id, userId);
                    return NotFound();
                }
                Log.Information("User {UserId} fetched todo with id {TodoId}", userId, id);
                return Ok(todo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching todo {TodoId} for user {UserId}", id, userId);
                return StatusCode(500, "An error occurred while fetching the todo.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddTodoDto dto)
        {
            int userId = GetUserIdFromToken();
            try
            {
                var created = await _service.AddAsync(dto, userId);
                Log.Information("User {UserId} added a new todo with id {TodoId}", userId, created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding todo for user {UserId}", userId);
                return StatusCode(500, "An error occurred while adding the todo.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoDto dto)
        {
            int userId = GetUserIdFromToken();
            try
            {
                var todo = await _service.UpdateAsync(id, dto, userId);
                if (todo == null)
                {
                    Log.Error("Todo with id {TodoId} not found for update by user {UserId}", id, userId);
                    return NotFound();
                }
                Log.Information("User {UserId} updated todo with id {TodoId}", userId, id);
                return Ok(todo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating todo {TodoId} for user {UserId}", id, userId);
                return StatusCode(500, "An error occurred while updating the todo.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserIdFromToken();
            try
            {
                var deleted = await _service.DeleteAsync(id, userId);
                if (!deleted)
                {
                    Log.Error("Todo with id {TodoId} not found for delete by user {UserId}", id, userId);
                    return NotFound();
                }
                Log.Information("User {UserId} deleted todo with id {TodoId}", userId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting todo {TodoId} for user {UserId}", id, userId);
                return StatusCode(500, "An error occurred while deleting the todo.");
            }
        }
    }
}