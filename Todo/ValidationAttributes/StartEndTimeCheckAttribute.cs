using System.ComponentModel.DataAnnotations;
using Todo.Dtos;

namespace Todo.ValidationAttributes
{
    public class StartEndTimeCheckAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dto = (TodoListPostDto)value;

            if (dto.StartTime == null || dto.EndTime == null)
            {
                return ValidationResult.Success;
            }

            if (dto.StartTime > dto.EndTime)
            {
                return new ValidationResult("開始時間不可以大於結束時間");
            }

            return ValidationResult.Success;
        }
    }
}
