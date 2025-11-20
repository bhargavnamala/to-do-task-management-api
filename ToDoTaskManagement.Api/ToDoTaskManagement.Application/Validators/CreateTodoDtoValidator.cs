using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Application.DTOs;

namespace ToDoTaskManagement.Application.Validators
{
    public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
    {
        public CreateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(500);
            RuleFor(x => x.Description).MaximumLength(2000);
        }
    }
}
