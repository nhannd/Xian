using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public class RequiredFieldValidator : Validator
    {
        private string _fieldDisplayName;
        private object _nullValue;

        public RequiredFieldValidator(TestValueCallbackDelegate testValueCallback, string fieldDisplayName, object nullValue)
            :base(testValueCallback)
        {
            _fieldDisplayName = fieldDisplayName;
            _nullValue = nullValue;
        }

        protected override ValidationResult Validate(object testValue)
        {
            if (AreEqual(_nullValue, testValue))
            {
                return new ValidationResult(false, string.Format("{0} is required", _fieldDisplayName));
            }
            else
            {
                return new ValidationResult(true, new string[0]);
            }
        }

        private bool AreEqual(object x, object y)
        {
            // if both are null, or both are the same object, they are equal
            if (System.Object.ReferenceEquals(x, y))
                return true;

            // if x is null, we already know from the first test that y is non-null, so they are not equal
            // otherwise, x is not null, so we can call x.Equals(y)
            return x != null && x.Equals(y);
        }
    }
}
