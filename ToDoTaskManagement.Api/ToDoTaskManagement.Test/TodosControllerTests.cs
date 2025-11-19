using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Api.Controllers;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Domain.Interfaces;

namespace ToDoTaskManagement.Test
{
    public class TodosControllerTests
    {
        private readonly Mock<ITodoService> _mockService;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _mockService = new Mock<ITodoService>();
            _controller = new TodosController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfTodos()
        {
            // Arrange
            var todos = new List<TodoDto> { new TodoDto { Id = 1, Title = "Test Todo" } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(todos);

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
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(todo);

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
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((TodoDto?)null);

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
            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdTodo, createdResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenTodoIsUpdated()
        {
            // Arrange
            var updateDto = new UpdateTodoDto { Title = "Updated Todo" };
            var updatedTodo = new TodoDto { Id = 1, Title = "Updated Todo" };
            _mockService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(updatedTodo);

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
            _mockService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync((TodoDto?)null);

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
            _mockService.Setup(s => s.ToggleAsync(1)).ReturnsAsync(toggledTodo);

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
            _mockService.Setup(s => s.ToggleAsync(1)).ReturnsAsync((TodoDto?)null);

            // Act
            var result = await _controller.Toggle(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenTodoIsDeleted()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
