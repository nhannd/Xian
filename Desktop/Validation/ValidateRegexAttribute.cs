using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearCanvas.Desktop.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateRegexAttribute : ValidationAttribute
    {
        private string _pattern;
        private RegexOptions _options;

        public ValidateRegexAttribute(string pattern, string failureMessage)
            :base(failureMessage)
        {
            _pattern = pattern;
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

        public override IValidationRule CreateRule(string propertyName, TestValueCallbackDelegate testValueCallback)
        {
            return new RegexRule(propertyName, testValueCallback, _pattern, _options, this.FailureMessage);
        }
    }
}
