using Microsoft.EntityFrameworkCore;
using ToDoTaskManagement.Data;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Infrastructure.Repositories;

namespace ToDoTaskManagement.Infrastructure.Test
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
        public async Task GetAllAsync_ReturnsTodosForUser()
        {
            // Arrange
            var userId = "user1";
            var todos = new List<TodoItem>
            {
                new TodoItem { Title = "Todo 1", UserId = userId, CreatedAt = DateTime.UtcNow },
                new TodoItem { Title = "Todo 2", UserId = userId, CreatedAt = DateTime.UtcNow.AddMinutes(1) },
                new TodoItem { Title = "Todo 3", UserId = "user2", CreatedAt = DateTime.UtcNow }
            };
            _dbContext.TodoItems.AddRange(todos);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(userId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(userId, t.UserId));
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTodo_WhenFound()
        {
            // Arrange
            var userId = "user1";
            var todo = new TodoItem { Title = "Test Todo", UserId = userId };
            _dbContext.TodoItems.Add(todo);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(todo.Id, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todo.Id, result.Id);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _repository.GetByIdAsync(1, "user1");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsTodoToDatabase()
        {
            // Arrange
            var todo = new TodoItem { Title = "New Todo", UserId = "user1" };

            // Act
            await _repository.AddAsync(todo);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _dbContext.TodoItems.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("New Todo", result.Title);
            Assert.Equal("user1", result.UserId);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTodoInDatabase()
        {
            // Arrange
            var todo = new TodoItem { Title = "Old Title", UserId = "user1" };
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
        public async Task DeleteAsync_RemovesTodoFromDatabase_WhenFound()
        {
            // Arrange
            var userId = "user1";
            var todo = new TodoItem { Title = "Test Todo", UserId = userId };
            _dbContext.TodoItems.Add(todo);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(todo.Id, userId);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _dbContext.TodoItems.FirstOrDefaultAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_DoesNothing_WhenTodoNotFound()
        {
            // Act
            await _repository.DeleteAsync(1, "user1");
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _dbContext.TodoItems.FirstOrDefaultAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveChangesAsync_SavesChangesToDatabase()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo", UserId = "user1" };
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