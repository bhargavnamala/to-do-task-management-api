using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoTaskManagement.Application.DTOs
{
    public class UpdateTodoDto
    {
        public required string Title { get; set; }
        public bool? IsCompleted { get; set; }
        public string? Description { get; set; }
    }
}
