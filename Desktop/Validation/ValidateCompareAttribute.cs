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

using System;
using System.Reflection;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Abstract base class for comparison validations.
	/// </summary>
	public abstract class ValidateCompareAttribute : ValidationAttribute
	{
		private string _referenceProperty;
		private object _referenceValue;
		private bool _inclusive;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceProperty">The name of another property to compare against.</param>
		protected ValidateCompareAttribute(string referenceProperty)
		{
			_referenceProperty = referenceProperty;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		protected ValidateCompareAttribute(int referenceValue)
		{
			_referenceValue = referenceValue;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		protected ValidateCompareAttribute(float referenceValue)
		{
			_referenceValue = referenceValue;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		protected ValidateCompareAttribute(double referenceValue)
		{
			_referenceValue = referenceValue;
		}

		/// <summary>
		/// Gets or sets whether or not the comparison should be inclusive.
		/// </summary>
		public bool Inclusive
		{
			get { return _inclusive; }
			set { _inclusive = value; }
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
			CheckPropertyIsType(property, typeof(IComparable));
			if (_referenceProperty != null)
			{
				// check that reference property is same type as validating property
				PropertyInfo refProp = property.DeclaringType.GetProperty(_referenceProperty);
				if (refProp.PropertyType != property.PropertyType)
					throw new ValidationAttributeException(SR.ExceptionReferencePropertyMustBeOfSameType);

				PropertyGetter refPropGetter = CreatePropertyGetter(refProp);
				return new ValidationRule(property.Name,
					delegate(IApplicationComponent c)
					{
						IComparable value = (IComparable)getter(c);
						object refValue = refPropGetter(c);
						return new ValidationResult(Compare(value, refValue, _inclusive), customMessage ?? GetDefaultMessage(refValue));
					});
			}
			else
			{
				// check that reference value is same type as validating property
				if (_referenceValue.GetType() != property.PropertyType)
					throw new ValidationAttributeException(SR.ExceptionReferenceValueMustBeOfSameType);

				// compare against fixed reference value
				return new ValidationRule(property.Name,
					delegate(IApplicationComponent c)
					{
						IComparable value = (IComparable)getter(c);
						return new ValidationResult(Compare(value, _referenceValue, _inclusive), customMessage ?? GetDefaultMessage(_referenceValue));
					});
			}
		}

		private bool Compare(IComparable x, object y, bool inclusive)
		{
			int i = GetCompareSign() * x.CompareTo(y);
			return i == 1 || (inclusive && i == 0);
		}

		private string GetDefaultMessage(object referenceValue)
		{
			if (GetCompareSign() > 0)
			{
				return _inclusive ?
					string.Format(SR.FormatMustBeGreaterThanOrEqualTo, referenceValue) :
					string.Format(SR.FormatMustBeGreaterThan, referenceValue);
			}
			else
			{
				return _inclusive ?
				   string.Format(SR.FormatMustBeLessThanOrEqualTo, referenceValue) :
				   string.Format(SR.FormatMustBeLessThan, referenceValue);
			}
		}

		/// <summary>
		/// Gets the sign of the comparison.
		/// </summary>
		protected abstract int GetCompareSign();
	}
}
