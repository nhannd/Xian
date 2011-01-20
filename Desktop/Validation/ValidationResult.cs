#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Represents the result of an <see cref="IValidationRule"/> evaluation.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Combines a collection of validation results into a single result.
        /// </summary>
        public static ValidationResult Combine(IEnumerable<ValidationResult> results)
        {
            List<string> messages = new List<string>();
            foreach (ValidationResult result in results)
            {
                if (!result.Success)
                    messages.AddRange(result.Messages);
            }

            return new ValidationResult(messages.Count == 0, messages.ToArray());
        }

        private bool _success;
        private string[] _messages;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="success">Indicates whether the validation succeeded.</param>
        /// <param name="message">When validation fails, a message indicating why the validation failed.</param>
        public ValidationResult(bool success, string message)
            : this(success, new string[] { message })
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="success">Indicates whether the validation succeeded.</param>
        /// <param name="messages">When validation fails, a set of messages indicating why the validation failed.</param>
        public ValidationResult(bool success, string[] messages)
        {
            _success = success;
            _messages = messages;
        }

        /// <summary>
        /// True if the validation was successful.
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }

        /// <summary>
        /// Messages that describe why validation was not successful.
        /// </summary>
        public string[] Messages
        {
            get { return _messages; }
        }

        /// <summary>
        /// Concatenates the elements of the <see cref="Messages"/> property into a single message using the specified separator.
        /// </summary>
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
}
