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

using System.Reflection;
using System.Text.RegularExpressions;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Validates a string property against a regular expression.
	/// </summary>
	public class ValidateRegexAttribute : ValidationAttribute
	{
		private string _pattern;
		private bool _allowNull;
		private bool _successOnMatch;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pattern">The regular expression pattern to test.</param>
		public ValidateRegexAttribute(string pattern)
		{
			_pattern = pattern;
			_successOnMatch = true;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not to allow a null/empty string.
		/// </summary>
		/// <remarks>
		/// If this property is set to true, a null/empty string is allowed even when the regex pattern would
		/// otherwise fail.  In this case, the regex is not even tested if the property value is null.
		/// This allows for non-required fields to be validated only if a value was in fact supplied.
		/// </remarks>
		public bool AllowNull
		{
			get { return _allowNull; }
			set { _allowNull = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether a successful match indicates a successful result.
		/// </summary>
		/// <remarks>
		/// For example, you may want to test that a string does not contain any 'a' or 'b' characters.  In order
		/// to do this, you must specify <see cref="SuccessOnMatch"/> = false and a pattern of "[ab]+".  This pattern
		/// will result in a successful match, and there is no easy way to do a !(not) in regular expressions.
		/// </remarks>
		public bool SuccessOnMatch
		{
			get { return _successOnMatch; }
			set { _successOnMatch = value; }
		}

		/// <summary>
		/// Factory method to create an <see cref="IValidationRule"/> based on this attribute.
		/// </summary>
		/// <param name="property">The property on which the attribute is applied.</param>
		/// <param name="getter">A delegate that, when invoked, returns the current value of the property.</param>
		/// <param name="customMessage">A custom message to be displayed, or null if none was supplied.</param>
		/// <returns></returns>
		protected override IValidationRule CreateRule(PropertyInfo property, PropertyGetter getter, string customMessage)
		{
			CheckPropertyIsType(property, typeof(string));
			string message = customMessage ?? SR.MessageUnrecognizedFormat;

			return new ValidationRule(property.Name,
				delegate(IApplicationComponent component)
				{
					string value = (string)getter(component);
					if (_allowNull && string.IsNullOrEmpty(value))
						return new ValidationResult(true, "");
					else
						return new ValidationResult(Regex.Match(value ?? "", _pattern).Success == SuccessOnMatch, message);
				});
		}
	}
}
