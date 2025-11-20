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
        public TodoRepository(TodoDbContext db) => _db = db;

        public Task<List<TodoItem>> GetAllAsync(string userId, CancellationToken ct = default) =>
            _db.TodoItems.Where(t => t.UserId == userId).AsNoTracking().OrderByDescending(t => t.CreatedAt).ToListAsync(ct);

        public Task<TodoItem?> GetByIdAsync(int id, string userId, CancellationToken ct = default) =>
            _db.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);

        public async Task AddAsync(TodoItem item, CancellationToken ct = default)
        {
            await _db.TodoItems.AddAsync(item, ct);
        }

        public Task UpdateAsync(TodoItem item, CancellationToken ct = default)
        {
            _db.TodoItems.Update(item);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, string userId, CancellationToken ct = default)
        {
            var task = await _db.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

           if(task != null) { 
                _db.TodoItems.Remove(task);
            }
        }

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
