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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Abstract base class for validation attributes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public abstract class ValidationAttribute : Attribute
	{
		/// <summary>
		/// Delegate used to get the value of a property from an <see cref="IApplicationComponent"/>.
		/// </summary>
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
}