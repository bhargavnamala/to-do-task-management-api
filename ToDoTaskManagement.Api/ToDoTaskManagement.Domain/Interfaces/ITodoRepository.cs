using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Domain.Entities;

namespace ToDoTaskManagement.Domain.Interfaces
{
    public interface ITodoRepository
    {
        Task<List<TodoItem>> GetAllAsync(string userId, CancellationToken ct = default);
        Task<TodoItem?> GetByIdAsync(int id, string userId, CancellationToken ct = default);
        Task AddAsync(TodoItem item, CancellationToken ct = default);
        Task UpdateAsync(TodoItem item, CancellationToken ct = default);
        Task DeleteAsync(int id, string userId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
