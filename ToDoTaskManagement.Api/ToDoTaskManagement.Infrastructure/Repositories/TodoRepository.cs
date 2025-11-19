using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Data;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Domain.Interfaces;

namespace ToDoTaskManagement.Infrastructure.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _db;

        public TodoRepository(TodoDbContext db)
        {
            _db = db;
        }

        public Task<List<TodoItem>> GetAllAsync()
            => _db.TodoItems.AsNoTracking().OrderBy(x => x.CreatedAt).ToListAsync();

        public Task<TodoItem?> GetByIdAsync(int id)
            => _db.TodoItems.FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(TodoItem item)
        {
            await _db.TodoItems.AddAsync(item);
        }

        public Task UpdateAsync(TodoItem item)
        {
            _db.TodoItems.Update(item);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TodoItem item)
        {
            _db.TodoItems.Remove(item);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();
    }
}
