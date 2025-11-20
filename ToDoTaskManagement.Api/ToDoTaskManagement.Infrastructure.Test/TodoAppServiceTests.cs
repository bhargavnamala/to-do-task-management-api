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

namespace ToDoTaskManagement.Infrastructure.Test
{
    public class TodoAppServiceTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly TodoAppService _service;

        public TodoAppServiceTests()
        {
            _mockRepo = new Mock<ITodoRepository>();
            _service = new TodoAppService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfTodos()
        {
            // Arrange
            var userId = "user1";
            var todos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Test Todo 1", UserId = userId },
                new TodoItem { Id = 2, Title = "Test Todo 2", UserId = userId }
            };
            _mockRepo.Setup(r => r.GetAllAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(todos);

            // Act
            var result = await _service.GetAllAsync(userId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Todo 1", result[0].Title);
            Assert.Equal("Test Todo 2", result[1].Title);
        }

        [Fact]
        public async Task GetAsync_ReturnsTodo_WhenFound()
        {
            // Arrange
            var userId = "user1";
            var todo = new TodoItem { Id = 1, Title = "Test Todo", UserId = userId };
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync(todo);

            // Act
            var result = await _service.GetAsync(1, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Todo", result.Title);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var userId = "user1";
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _service.GetAsync(1, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_AddsTodoAndReturnsCreatedTodo()
        {
            // Arrange
            var userId = "user1";
            var createDto = new CreateTodoDto { Title = "New Todo", Description = "Description" };
            var createdTodo = new TodoItem { Id = 1, Title = "New Todo", Description = "Description", UserId = userId };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
                     .Callback<TodoItem, CancellationToken>((t, _) => t.Id = 1);
            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(userId, createDto);

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
            var userId = "user1";
            var existingTodo = new TodoItem { Id = 1, Title = "Old Title", UserId = userId, Description = "Old Description" };
            var updateDto = new UpdateTodoDto { Title = "Updated Title", Description = "Updated Description", IsCompleted = true };
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync(existingTodo);
            _mockRepo.Setup(r => r.UpdateAsync(existingTodo, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(1, userId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Updated Description", result.Description);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenTodoNotFound()
        {
            // Arrange
            var userId = "user1";
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _service.UpdateAsync(1, userId, new UpdateTodoDto());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_DeletesTodoAndReturnsTrue()
        {
            // Arrange
            var userId = "user1";
            var todo = new TodoItem { Id = 1, Title = "Test Todo", UserId = userId };
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync(todo);
            _mockRepo.Setup(r => r.DeleteAsync(1, userId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenTodoNotFound()
        {
            // Arrange
            var userId = "user1";
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _service.DeleteAsync(1, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ToggleAsync_TogglesTodoCompletionStatus()
        {
            // Arrange
            var userId = "user1";
            var todo = new TodoItem { Id = 1, Title = "Test Todo", UserId = userId, IsCompleted = false };
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync(todo);
            _mockRepo.Setup(r => r.UpdateAsync(todo, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ToggleAsync(1, userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public async Task ToggleAsync_ReturnsNull_WhenTodoNotFound()
        {
            // Arrange
            var userId = "user1";
            _mockRepo.Setup(r => r.GetByIdAsync(1, userId, It.IsAny<CancellationToken>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _service.ToggleAsync(1, userId);

            // Assert
            Assert.Null(result);
        }
    }
}