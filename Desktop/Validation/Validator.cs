using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    public class ValidatorResult
    {
        private bool _isValid;
        private string _message;

        public ValidatorResult(bool isValid, string message)
        {
            _isValid = isValid;
            _message = message;
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public string Message
        {
            get { return _message; }
        }
    }

    public abstract class ValidatorBase
    {
    }

    public class Validator : ValidatorBase
    {
        public delegate object GetPropertyValueDelegate();

        private bool _mandatory;
        private string _displayName;
        private GetPropertyValueDelegate _propertyGetter;

        public Validator(string displayName, GetPropertyValueDelegate getter, bool mandatory)
        {
            _displayName = displayName;
            _mandatory = mandatory;
            _propertyGetter = getter;
        }

        public ValidatorResult Result
        {
            get
            {
                if (_propertyGetter() == null)
                {
                    return new ValidatorResult(false, string.Format("{0} is required", _displayName));
                }
                else
                {
                    return new ValidatorResult(true, null);
                }
            }
        }
    }
}
