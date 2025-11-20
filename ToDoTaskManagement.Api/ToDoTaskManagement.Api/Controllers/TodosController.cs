using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Application.Interfaces;

namespace ToDoTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly ITodoAppService _app;
        private readonly ILogger<TodosController> _logger;

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        public TodosController(
            ITodoAppService app,
            ILogger<TodosController> logger)
        {
            _app = app;
            _logger = logger;
        }

        // GET: api/todos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching todo list for user {UserId}", CurrentUserId);

            var list = await _app.GetAllAsync(CurrentUserId);
            return Ok(list);
        }

        // GET: api/todos/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Fetching todo {TodoId} for user {UserId}", id, CurrentUserId);

            var todo = await _app.GetAsync(id, CurrentUserId);
            if (todo == null)
            {
                _logger.LogWarning("Todo {TodoId} not found for user {UserId}", id, CurrentUserId);
                return NotFound();
            }

            return Ok(todo);
        }

        // POST: api/todos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid CreateTodoDto received");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating todo for user {UserId}", CurrentUserId);

            var created = await _app.CreateAsync(CurrentUserId, dto);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // PUT: api/todos/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid UpdateTodoDto received for todo {TodoId}", id);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating todo {TodoId} for user {UserId}", id, CurrentUserId);

            var updated = await _app.UpdateAsync(id, CurrentUserId, dto);
            if (updated == null)
            {
                _logger.LogWarning("Todo {TodoId} not found during update", id);
                return NotFound();
            }

            return Ok(updated);
        }

        // PATCH: api/todos/{id}/toggle
        [HttpPatch("{id:int}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            _logger.LogInformation("Toggling todo {TodoId} for user {UserId}", id, CurrentUserId);

            var toggled = await _app.ToggleAsync(id, CurrentUserId);

            if (toggled == null)
            {
                _logger.LogWarning("Todo {TodoId} not found for toggle operation", id);
                return NotFound();
            }

            return Ok(toggled);
        }

        // DELETE: api/todos/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting todo {TodoId} for user {UserId}", id, CurrentUserId);

            var success = await _app.DeleteAsync(id, CurrentUserId);

            if (!success)
            {
                _logger.LogWarning("Todo {TodoId} not found for delete operation", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}