using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Application.Interfaces;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Domain.Interfaces;

namespace ToDoTaskManagement.Infrastructure.Services
{
    public class TodoAppService : ITodoAppService
    {
        private readonly ITodoRepository _repo;
        public TodoAppService(ITodoRepository repo) => _repo = repo;

        private static TodoDto Map(TodoItem e) =>
            new TodoDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                IsCompleted = e.IsCompleted,
                CreatedAt = e.CreatedAt
            };

        public async Task<List<TodoDto>> GetAllAsync(string userId,CancellationToken ct = default)
        {
            var items = await _repo.GetAllAsync(userId, ct);
            return items.Select(Map).ToList();
        }

        public async Task<TodoDto?> GetAsync(int id, string userId,CancellationToken ct = default)
        {
            var item = await _repo.GetByIdAsync(id, userId, ct);
            return item == null ? null : Map(item);
        }

        public async Task<TodoDto> CreateAsync(string userId, CreateTodoDto dto, CancellationToken ct = default)
        {
            var entity = new TodoItem { Title = dto.Title.Trim(), Description = dto.Description, UserId = userId};
            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);
            return Map(entity);
        }

        public async Task<TodoDto?> UpdateAsync(int id, string userId, UpdateTodoDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, userId, ct);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Title)) entity.Title = dto.Title.Trim();
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.IsCompleted.HasValue) entity.IsCompleted = dto.IsCompleted.Value;

            await _repo.UpdateAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, userId, ct);
            if (entity == null) return false;
            await _repo.DeleteAsync(id, userId, ct);
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<TodoDto?> ToggleAsync(int id, string userId, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, userId, ct);
            if (entity == null) return null;
            entity.IsCompleted = !entity.IsCompleted;
            await _repo.UpdateAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);
            return Map(entity);
        }
    }
}
