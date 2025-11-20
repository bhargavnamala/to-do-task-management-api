using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Application.DTOs;

namespace ToDoTaskManagement.Application.Interfaces
{
    public interface ITodoAppService
    {
        Task<List<TodoDto>> GetAllAsync(string userId, CancellationToken ct = default);
        Task<TodoDto?> GetAsync(int id, string userId, CancellationToken ct = default);
        Task<TodoDto> CreateAsync(string userId, CreateTodoDto dto, CancellationToken ct = default);
        Task<TodoDto?> UpdateAsync(int id, string userId, UpdateTodoDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default);
        Task<TodoDto?> ToggleAsync(int id, string userId, CancellationToken ct = default);
    }
}
