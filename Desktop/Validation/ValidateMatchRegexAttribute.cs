using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearCanvas.Desktop.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateMatchRegexAttribute : ValidationAttribute
    {
        private string _pattern;
        private string _failureMessage;
        private RegexOptions _options;

        public ValidateMatchRegexAttribute(string pattern, string failureMessage)
        {
            _pattern = pattern;
            _failureMessage = failureMessage;
        }

        public RegexOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        public string Pattern
        {
            get { return _pattern; }
        }

        public string FailureMessage
        {
            get { return _failureMessage; }
        }

        public override IValidator CreateValidator(string propertyName, TestValueCallbackDelegate testValueCallback)
        {
            return new RegexValidator(propertyName, testValueCallback, _pattern, _options, _failureMessage);
        }
    }
}
