using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{

    public delegate object TestValueCallbackDelegate();

    public abstract class ValidationRule : IValidationRule
    {
        private string _propertyName;
        private TestValueCallbackDelegate _testValueCallback;

        public ValidationRule(string propertyName, TestValueCallbackDelegate testValueCallback)
        {
            _propertyName = propertyName;
            _testValueCallback = testValueCallback;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public ValidationResult Result
        {
            get
            {
                return Validate(_testValueCallback());
            }
        }

        protected abstract ValidationResult Validate(object testValue);
    }
}
