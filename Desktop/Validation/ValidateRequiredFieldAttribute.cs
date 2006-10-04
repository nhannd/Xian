using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateRequiredFieldAttribute : ValidationAttribute
    {
        private string _fieldDisplayName;

        public ValidateRequiredFieldAttribute(string fieldDisplayName)
        {
            _fieldDisplayName = fieldDisplayName;
        }

        public string FieldDisplayName
        {
            get { return _fieldDisplayName; }
        }

        public override IValidator CreateValidator(TestValueCallbackDelegate testValueCallback)
        {
            return new RequiredFieldValidator(testValueCallback, _fieldDisplayName, null);
        }
    }
}
