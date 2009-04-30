#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
