using System.ComponentModel.DataAnnotations;
using System.Linq;
using Todo.Models;

namespace Todo.ValidationAttributes
{
    public class TodoNameCheckAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var todoContext = (TodoContext)validationContext.GetService(typeof(TodoContext));
            var name = (string)value;

            if (todoContext.TodoLists.Any(x => x.Name == name))
            {
                return new ValidationResult("已存在相同的代辦事項");
            }

            return base.IsValid(value, validationContext);
        }
    }
}
