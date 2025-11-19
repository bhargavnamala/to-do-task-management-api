using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Domain.Entities;

namespace ToDoTaskManagement.Domain.Interfaces
{
    public interface ITodoService
    {
        Task<List<TodoDto>> GetAllAsync();
        Task<TodoDto?> GetByIdAsync(int id);
        Task<TodoDto> CreateAsync(CreateTodoDto dto);
        Task<TodoDto?> UpdateAsync(int id, UpdateTodoDto dto);
        Task<bool> DeleteAsync(int id);
        Task<TodoDto?> ToggleAsync(int id);
    }
}
