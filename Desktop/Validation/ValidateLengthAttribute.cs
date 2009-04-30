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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Validates that the length of a string property is within a specified range.
	/// </summary>
	/// <remarks>
	/// This attribute offers a simpler alternative to the <see cref="ValidateRegexAttribute"/> when
	/// all you want to do is validate the length of the string.
	/// </remarks>
	public class ValidateLengthAttribute : ValidationAttribute
	{
		private int _minLength;
		private int _maxLength;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ValidateLengthAttribute(int minLength)
			: this(minLength, -1)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ValidateLengthAttribute(int minLength, int maxLength)
		{
			Platform.CheckArgumentRange(minLength, 0, int.MaxValue, "minLength");
			Platform.CheckArgumentRange(maxLength, -1, int.MaxValue, "maxLength");

			_minLength = minLength;
			_maxLength = maxLength;
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
			string message = customMessage;
			if (message == null)
			{
				if (_maxLength == -1)
					message = string.Format(SR.FormatMoreThanXCharactersRequired, _minLength);
				else
					message = string.Format(SR.FormatRangeCharactersRequired, _minLength, _maxLength);
			}

			return new ValidationRule(property.Name,
			   delegate(IApplicationComponent component)
			   {
				   // convert null string to ""
				   string value = ((string)getter(component)) ?? "";
				   return new ValidationResult(value.Length >= _minLength && (_maxLength == -1 || value.Length <= _maxLength), message);
			   });
		}
	}
}
