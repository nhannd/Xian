using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public abstract class ValidationAttribute : Attribute
    {
        private string _failureMessage;

        public ValidationAttribute(string failureMessage)
        {
            _failureMessage = failureMessage;
        }

        public string FailureMessage
        {
            get { return _failureMessage; }
        }

        public abstract IValidationRule CreateRule(string propertyName, TestValueCallbackDelegate testValueCallback);
    }
}
