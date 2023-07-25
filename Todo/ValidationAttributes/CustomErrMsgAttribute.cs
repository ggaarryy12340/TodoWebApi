using System.ComponentModel.DataAnnotations;

namespace Todo.ValidationAttributes
{
    public class CustomErrMsgAttribute: ValidationAttribute
    {
        private bool _isValid;
        private string _errMsg;
        public CustomErrMsgAttribute(bool isValid, string errMsg)
        { 
            _isValid = isValid;
            _errMsg = errMsg;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (_isValid)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(_errMsg);
        }
    }
}
