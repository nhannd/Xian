using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearCanvas.Desktop.Validation
{
    public class RegexValidator : Validator
    {
        private Regex _regex;
        private string _failureMessage;

        public RegexValidator(string propertyName, TestValueCallbackDelegate testValueCallback, Regex regex, string failureMessage)
            :base(propertyName, testValueCallback)
        {
            _regex = regex;
            _failureMessage = failureMessage;
        }

        public RegexValidator(string propertyName, TestValueCallbackDelegate testValueCallback, string pattern, string failureMessage)
            : this(propertyName, testValueCallback, pattern, RegexOptions.None, failureMessage)
        {
        }

        public RegexValidator(string propertyName, TestValueCallbackDelegate testValueCallback, string pattern, RegexOptions options, string failureMessage)
            : this(propertyName, testValueCallback, new Regex(pattern, options), failureMessage)
        {
        }

        protected override ValidationResult Validate(object testValue)
        {
            if (Match(testValue))
            {
                return new ValidationResult(true, new string[0]);
            }
            else
            {
                return new ValidationResult(false, _failureMessage);
            }
        }

        private bool Match(object testValue)
        {
            // if the value is null, we have to assume it is valid, since we can't assume it is a required field
            if (testValue == null)
            {
                return true;
            }

            string text = testValue as string;
            if (text == null)
            {
                throw new ArgumentException("Test value must be a string.");
            }

            // if the text is empty, we have to assume it is valid, since we can't assume it is a required field
            if (text.Length == 0)
            {
                return true;
            }

            // test against the regular expression pattern
            Match match = _regex.Match(text);
            return match.Success && match.Index == 0 && match.Length == text.Length;
        }
    }
}
