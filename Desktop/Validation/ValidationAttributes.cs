#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;
using System.Text.RegularExpressions;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Abstract base class for validation attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true, Inherited=true)]
    public abstract class ValidationAttribute : Attribute
    {
        public delegate object PropertyGetter(IApplicationComponent component);

        private string _message;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ValidationAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the custom message to be displayed when a validation error occurs.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Gets the custom message localized according to the specified resource resolver.
        /// </summary>
        protected string GetLocalizedCustomMessage(IResourceResolver resourceResolver)
        {
            return string.IsNullOrEmpty(_message) ? null : resourceResolver.LocalizeString(_message);
        }

        /// <summary>
        /// Validates that the specified property is assignable to one of the specified types.
        /// </summary>
        protected void CheckPropertyIsType(PropertyInfo property, params Type[] types)
        {
            if (!CollectionUtils.Contains<Type>(types, delegate(Type t) { return t.IsAssignableFrom(property.PropertyType); }))
                throw new ValidationAttributeException(
					string.Format(SR.FormatAttributeCannotBeAppliedToPropertyType, this.GetType().Name, property.PropertyType.FullName));
        }

        /// <summary>
        /// Factory method to create a delegate that invokes the getter of the specified property.
        /// </summary>
        protected PropertyGetter CreatePropertyGetter(PropertyInfo property)
        {
            //JR> in theory we should be able to bind a delegate to the property's GetMethod,
            //which would perform better than reflection for repeated invocations,
            //but for some reason it fails
            //MethodInfo propertyGetter = property.GetGetMethod();
            //return (PropertyGetter)Delegate.CreateDelegate(typeof(PropertyGetter), null, propertyGetter);
            
            // oh well, too bad for performance - invoke via reflection
            return new PropertyGetter(
                delegate(IApplicationComponent component)
                {
                    return property.GetGetMethod().Invoke(component, null);
                });
        }

        /// <summary>
        /// Factory method to create an <see cref="IValidationRule"/> based on this attribute.
        /// </summary>
        public IValidationRule CreateRule(PropertyInfo property, IResourceResolver resourceResolver)
        {
            PropertyGetter getter = CreatePropertyGetter(property);
            string customMessage = GetLocalizedCustomMessage(resourceResolver);
            return CreateRule(property, getter, customMessage);
        }

        /// <summary>
        /// Factory method to create an <see cref="IValidationRule"/> based on this attribute.
        /// </summary>
        /// <param name="property">The property on which the attribute is applied.</param>
        /// <param name="getter">A delegate that, when invoked, returns the current value of the property.</param>
        /// <param name="customMessage">A custom message to be displayed, or null if none was supplied.</param>
        /// <returns></returns>
        protected abstract IValidationRule CreateRule(PropertyInfo property, PropertyGetter getter, string customMessage);
    }

    /// <summary>
    /// Validates a string property against a regular expression.
    /// </summary>
    public class ValidateRegexAttribute : ValidationAttribute
    {
        private string _pattern;
        private bool _allowNull;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to test.</param>
        public ValidateRegexAttribute(string pattern)
        {
            _pattern = pattern;
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
                        return new ValidationResult(Regex.Match(value ?? "", _pattern).Success, message);
                });
        }
    }

    /// <summary>
    /// Validates that a property is not null.
    /// </summary>
    public class ValidateNotNullAttribute : ValidationAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateNotNullAttribute()
        {
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
			string message = customMessage ?? SR.MessageValueRequired;
            return new ValidationRule(property.Name,
               delegate(IApplicationComponent component)
               {
                   object value = getter(component);
                   return new ValidationResult((value is string) ? !string.IsNullOrEmpty(value as string) : value != null, message);
               });
        }
    }

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

	//TODO (Jon): actually this won't work because there is no data-binding to an ITable/IEnumerable property,
	//hence nowhere to display the error symbol on the UI
	//leave as internal for now until we figure out how to resolve this
	
	/// <summary>
    /// Validates that the number of items in an <see cref="IEnumerable"/> property is within a specified range.
    /// </summary>
    /// <remarks>
    /// Can also be applied to properties of type <see cref="ITable"/>, in which case it is applied to the
    /// <see cref="ITable.Items"/> collection.
	/// </remarks>
    internal class ValidateCountAttribute : ValidationAttribute
    {
        private int _min;
        private int _max;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateCountAttribute(int min, int max)
        {
            Platform.CheckArgumentRange(min, 0, int.MaxValue, "min");
            Platform.CheckArgumentRange(max, -1, int.MaxValue, "max");

            _min = min;
            _max = max;
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
            CheckPropertyIsType(property, typeof(ITable), typeof(IEnumerable));

            return new ValidationRule(property.Name,
               delegate(IApplicationComponent component)
               {
                   IEnumerable value = null;
                   if (value is ITable)
                       value = ((ITable)getter(component)).Items;
                   else
                       value = (IEnumerable)getter(component);

                   int count = 0;
                   if(value is IList)
                       count = (value as IList).Count;
                   else if(value is ICollection)
                       count = (value as ICollection).Count;
                   else
                   {
                       // enumerate the items to count them
                       foreach(object obj in value)
                           count++;
                   }

                   return new ValidationResult(count >= _min && (_max == -1 || count <= _max), customMessage);
               });
        }
    }

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
		public ValidateCompareAttribute(string referenceProperty)
        {
            _referenceProperty = referenceProperty;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		public ValidateCompareAttribute(int referenceValue)
        {
            _referenceValue = referenceValue;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		public ValidateCompareAttribute(float referenceValue)
        {
            _referenceValue = referenceValue;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		public ValidateCompareAttribute(double referenceValue)
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
            int i = GetCompareSign()*x.CompareTo(y);
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

    /// <summary>
    /// Validates that a property value is greater than a reference value.
    /// </summary>
    public class ValidateGreaterThanAttribute : ValidateCompareAttribute
    {
        /// <summary>
        /// Constructor that accepts the name of a reference property.
        /// </summary>
        /// <param name="referenceProperty">The name of a property on the component that provides a reference value.</param>
        public ValidateGreaterThanAttribute(string referenceProperty)
            :base(referenceProperty)
        {
        }

        /// <summary>
        /// Constructor that accepts a constant reference value.
        /// </summary>
        public ValidateGreaterThanAttribute(int referenceValue)
            : base(referenceValue)
        {
        }

        /// <summary>
        /// Constructor that accepts a constant reference value.
        /// </summary>
        public ValidateGreaterThanAttribute(float referenceValue)
            : base(referenceValue)
        {
        }

        /// <summary>
        /// Constructor that accepts a constant reference value.
        /// </summary>
        public ValidateGreaterThanAttribute(double referenceValue)
            : base(referenceValue)
        {
        }

        /// <summary>
        /// Returns 1.
        /// </summary>
		protected override int GetCompareSign()
        {
            return 1;
        }
    }

    /// <summary>
    /// Validates that a property value is less than reference value.
    /// </summary>
    public class ValidateLessThanAttribute : ValidateCompareAttribute
    {
        /// <summary>
        /// Constructor that accepts the name of a reference property.
        /// </summary>
        /// <param name="referenceProperty">The name of a property on the component that provides a reference value.</param>
        public ValidateLessThanAttribute(string referenceProperty)
            : base(referenceProperty)
        {
        }

        /// <summary>
        /// Constructor that accepts a constant reference value.
        /// </summary>
        public ValidateLessThanAttribute(int referenceValue)
            : base(referenceValue)
        {
        }

        /// <summary>
        /// Constructor that accepts a constant reference value.
        /// </summary>
        public ValidateLessThanAttribute(float referenceValue)
            : base(referenceValue)
        {
        }

        /// <summary>
        /// Constructor that accepts a constant reference value.
        /// </summary>
        public ValidateLessThanAttribute(double referenceValue)
            : base(referenceValue)
        {
        }

		/// <summary>
		/// Returns -1.
		/// </summary>
        protected override int GetCompareSign()
        {
            return -1;
        }
    }
}
