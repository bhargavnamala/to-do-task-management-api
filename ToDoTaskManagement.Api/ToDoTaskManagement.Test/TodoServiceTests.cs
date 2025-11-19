using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Domain.Interfaces;
using ToDoTaskManagement.Infrastructure.Services;

namespace ToDoTaskManagement.Test
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _mockRepo = new Mock<ITodoRepository>();
            _service = new TodoService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfTodos()
        {
            // Arrange
            var todos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Test Todo 1" },
                new TodoItem { Id = 2, Title = "Test Todo 2" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(todos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Todo 1", result[0].Title);
            Assert.Equal("Test Todo 2", result[1].Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTodo_WhenFound()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Title = "Test Todo" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Todo", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_AddsTodoAndReturnsCreatedTodo()
        {
            // Arrange
            var createDto = new CreateTodoDto { Title = "New Todo", Description = "Description" };
            var createdTodo = new TodoItem { Id = 1, Title = "New Todo", Description = "Description" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<TodoItem>())).Callback<TodoItem>(t => t.Id = 1);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Todo", result.Title);
            Assert.Equal("Description", result.Description);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTodoAndReturnsUpdatedTodo()
        {
            // Arrange
            var existingTodo = new TodoItem { Id = 1, Title = "Old Title", IsCompleted = false, Description = "Old Description" };
            var updateDto = new UpdateTodoDto { Title = "Updated Title", IsCompleted = true, Description = "Updated Description" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingTodo);
            _mockRepo.Setup(r => r.UpdateAsync(existingTodo)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.True(result.IsCompleted);
            Assert.Equal("Updated Description", result.Description);
        }

        [Fact]
        public async Task DeleteAsync_DeletesTodoAndReturnsTrue()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Title = "Test Todo" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);
            _mockRepo.Setup(r => r.DeleteAsync(todo)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenTodoNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ToggleAsync_TogglesTodoCompletionStatus()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);
            _mockRepo.Setup(r => r.UpdateAsync(todo)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ToggleAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public async Task ToggleAsync_ReturnsNull_WhenTodoNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _service.ToggleAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
