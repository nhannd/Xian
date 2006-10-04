using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    public class ValidationResult
    {
        private bool _isValid;
        private string[] _messages;

        public ValidationResult(bool isValid, string message)
            : this(isValid, new string[] { message })
        {
        }

        public ValidationResult(bool isValid, string[] messages)
        {
            _isValid = isValid;
            _messages = messages;
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public string[] Messages
        {
            get { return _messages; }
        }

        public string GetMessageString(string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string msg in _messages)
            {
                if (sb.Length > 0)
                    sb.Append(separator);
                sb.Append(msg);
            }
            return sb.ToString();
        }
    }

    public delegate object TestValueCallbackDelegate();

    public abstract class Validator : IValidator
    {
        private TestValueCallbackDelegate _testValueCallback;

        public Validator(TestValueCallbackDelegate testValueCallback)
        {
            _testValueCallback = testValueCallback;
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
