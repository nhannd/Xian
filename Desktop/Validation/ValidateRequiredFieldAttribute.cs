using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateRequiredFieldAttribute : ValidationAttribute
    {
        public ValidateRequiredFieldAttribute(string failureMessage)
            :base(failureMessage)
        {
        }

        public override IValidationRule CreateRule(string propertyName, TestValueCallbackDelegate testValueCallback)
        {
            return new RequiredFieldRule(propertyName, testValueCallback, this.FailureMessage, null);
        }
    }
}
