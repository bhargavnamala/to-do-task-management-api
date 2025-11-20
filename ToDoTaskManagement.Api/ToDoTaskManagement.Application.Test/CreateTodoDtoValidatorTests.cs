using FluentValidation.TestHelper;
using ToDoTaskManagement.Application.DTOs;
using ToDoTaskManagement.Application.Validators;

namespace ToDoTaskManagement.Application.Test
{
    public class CreateTodoDtoValidatorTests
    {
        private readonly CreateTodoDtoValidator _validator;

        public CreateTodoDtoValidatorTests()
        {
            _validator = new CreateTodoDtoValidator();
        }

        [Fact]
        public void Validate_TitleIsEmpty_ReturnsValidationError()
        {
            // Arrange
            var dto = new CreateTodoDto { Title = "", Description = "Valid Description" };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required");
        }

        [Fact]
        public void Validate_TitleExceedsMaxLength_ReturnsValidationError()
        {
            // Arrange
            var dto = new CreateTodoDto { Title = new string('A', 501), Description = "Valid Description" };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("The length of 'Title' must be 500 characters or fewer. You entered 501 characters.");
        }

        [Fact]
        public void Validate_DescriptionExceedsMaxLength_ReturnsValidationError()
        {
            // Arrange
            var dto = new CreateTodoDto { Title = "Valid Title", Description = new string('A', 2001) };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage("The length of 'Description' must be 2000 characters or fewer. You entered 2001 characters.");
        }

        [Fact]
        public void Validate_ValidDto_ReturnsNoValidationErrors()
        {
            // Arrange
            var dto = new CreateTodoDto { Title = "Valid Title", Description = "Valid Description" };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}