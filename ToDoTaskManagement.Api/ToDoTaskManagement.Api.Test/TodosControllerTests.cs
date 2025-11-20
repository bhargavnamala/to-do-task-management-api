using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using ToDoTaskManagement.Api.Controllers;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Application.Interfaces;

namespace ToDoTaskManagement.Api.Test
{
    public class TodosControllerTests
    {
        private readonly Mock<ITodoAppService> _mockAppService;
        private readonly Mock<ILogger<TodosController>> _mockLogger;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _mockAppService = new Mock<ITodoAppService>();
            _mockLogger = new Mock<ILogger<TodosController>>();

            _controller = new TodosController(_mockAppService.Object, _mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Set a mock user with a NameIdentifier claim
            _controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            }));
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfTodos()
        {
            // Arrange
            var todos = new List<TodoDto> { new TodoDto { Id = 1, Title = "Test Todo" } };
            _mockAppService.Setup(s => s.GetAllAsync("user1", default)).ReturnsAsync(todos);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(todos, okResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WhenTodoExists()
        {
            // Arrange
            var todo = new TodoDto { Id = 1, Title = "Test Todo" };
            _mockAppService.Setup(s => s.GetAsync(1, "user1", default)).ReturnsAsync(todo);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(todo, okResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockAppService.Setup(s => s.GetAsync(1, "user1", default)).ReturnsAsync((TodoDto?)null);

            // Act
            var result = await _controller.Get(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenTodoIsCreated()
        {
            // Arrange
            var createDto = new CreateTodoDto { Title = "New Todo" };
            var createdTodo = new TodoDto { Id = 1, Title = "New Todo" };
            _mockAppService.Setup(s => s.CreateAsync("user1", createDto, default)).ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdTodo, createdResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenTodoIsUpdated()
        {
            // Arrange
            var updateDto = new UpdateTodoDto { Title = "Updated Todo" };
            var updatedTodo = new TodoDto { Id = 1, Title = "Updated Todo" };
            _mockAppService.Setup(s => s.UpdateAsync(1, "user1", updateDto, default)).ReturnsAsync(updatedTodo);

            // Act
            var result = await _controller.Update(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedTodo, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateTodoDto { Title = "Updated Todo" };
            _mockAppService.Setup(s => s.UpdateAsync(1, "user1", updateDto, default)).ReturnsAsync((TodoDto?)null);

            // Act
            var result = await _controller.Update(1, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Toggle_ReturnsOkResult_WhenTodoIsToggled()
        {
            // Arrange
            var toggledTodo = new TodoDto { Id = 1, Title = "Test Todo", IsCompleted = true };
            _mockAppService.Setup(s => s.ToggleAsync(1, "user1", default)).ReturnsAsync(toggledTodo);

            // Act
            var result = await _controller.Toggle(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(toggledTodo, okResult.Value);
        }

        [Fact]
        public async Task Toggle_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockAppService.Setup(s => s.ToggleAsync(1, "user1", default)).ReturnsAsync((TodoDto?)null);

            // Act
            var result = await _controller.Toggle(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenTodoIsDeleted()
        {
            // Arrange
            _mockAppService.Setup(s => s.DeleteAsync(1, "user1", default)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockAppService.Setup(s => s.DeleteAsync(1, "user1", default)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}