using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Domain.Interfaces;

namespace ToDoTaskManagement.Infrastructure.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repo;

        public TodoService(ITodoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<TodoDto>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<TodoDto?> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<TodoDto> CreateAsync(CreateTodoDto dto)
        {
            var entity = new TodoItem
            {
                Title = dto.Title?.Trim() ?? string.Empty,
                Description = dto.Description,
                IsCompleted = false
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<TodoDto?> UpdateAsync(int id, UpdateTodoDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Title)) entity.Title = dto.Title.Trim();
            if (dto.IsCompleted.HasValue) entity.IsCompleted = dto.IsCompleted.Value;
            entity.Description = dto.Description;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            await _repo.DeleteAsync(entity);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<TodoDto?> ToggleAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            entity.IsCompleted = !entity.IsCompleted;
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }

        private static TodoDto MapToDto(TodoItem e) =>
            new TodoDto
            {
                Id = e.Id,
                Title = e.Title,
                IsCompleted = e.IsCompleted,
                Description = e.Description,
                CreatedAt = e.CreatedAt
            };
    }
}
