using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Data;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Infrastructure.Repositories;

namespace ToDoTaskManagement.Test
{
    public class TodoRepositoryTests : IDisposable
    {
        private readonly TodoDbContext _dbContext;
        private readonly TodoRepository _repository;

        public TodoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TodoDbContext(options);
            _repository = new TodoRepository(_dbContext);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTodos()
        {
            // Arrange
            var todos = new List<TodoItem>
            {
                new TodoItem { Title = "Test Todo 1", CreatedAt = DateTime.UtcNow },
                new TodoItem { Title = "Test Todo 2", CreatedAt = DateTime.UtcNow.AddMinutes(1) }
            };
            _dbContext.TodoItems.AddRange(todos);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Todo 1", result[0].Title);
            Assert.Equal("Test Todo 2", result[1].Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTodo_WhenFound()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo" };
            _dbContext.TodoItems.Add(todo);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(todo.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todo.Id, result.Id);
            Assert.Equal("Test Todo", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsTodoToDatabase()
        {
            // Arrange
            var todo = new TodoItem { Title = "New Todo" };

            // Act
            await _repository.AddAsync(todo);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _dbContext.TodoItems.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("New Todo", result.Title);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTodoInDatabase()
        {
            // Arrange
            var todo = new TodoItem { Title = "Old Title" };
            _dbContext.TodoItems.Add(todo);
            await _dbContext.SaveChangesAsync();

            // Act
            todo.Title = "Updated Title";
            await _repository.UpdateAsync(todo);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _dbContext.TodoItems.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
        }

        [Fact]
        public async Task DeleteAsync_RemovesTodoFromDatabase()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo" };
            _dbContext.TodoItems.Add(todo);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(todo);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _dbContext.TodoItems.FirstOrDefaultAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveChangesAsync_SavesChangesToDatabase()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo" };
            _dbContext.TodoItems.Add(todo);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _dbContext.TodoItems.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("Test Todo", result.Title);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
