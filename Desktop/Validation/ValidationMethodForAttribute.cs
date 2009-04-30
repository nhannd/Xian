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
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Thrown by <see cref="ValidationMethodForAttribute"/>.
	/// </summary>
	public class ValidationMethodForAttributeException : Exception
	{
		internal ValidationMethodForAttributeException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Attribute used to decorate a method as a validation method.
	/// </summary>
	/// <remarks>
	/// The property matching <see cref="PropertyName"/> will be validated
	/// via the decorated method.  The method must match the signature of
	/// <see cref="ValidationMethod"/>.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ValidationMethodForAttribute : Attribute
	{
		/// <summary>
		/// Defines the method signature for methods decorated with <see cref="ValidationMethodForAttribute"/>.
		/// </summary>
		public delegate ValidationResult ValidationMethod();

		private readonly string _propertyName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The property the decorated method is intended to validate.</param>
		public ValidationMethodForAttribute(string propertyName)
		{
			_propertyName = propertyName;
		}

		/// <summary>
		/// Identifies the property whose validation will be done via the decorated method.
		/// </summary>
		public string PropertyName
		{
			get { return _propertyName; }	
		}

		private static void CheckMethodSignature(MethodInfo method)
		{
			if (!(typeof(ValidationResult).IsAssignableFrom(method.ReturnType)))
				throw new ValidationAttributeException("The decorated method does not have the correct signature.");

			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != 0)
				throw new ValidationAttributeException("The decorated method does not have the correct signature.");
		}

		/// <summary>
		/// Factory method that creates an <see cref="IValidationRule"/> for 
		/// the property with the name <see cref="PropertyName"/>.
		/// </summary>
		internal IValidationRule CreateRule(MethodInfo method)
		{
			CheckMethodSignature(method);

			return new ValidationRule(_propertyName,
			                          delegate(IApplicationComponent component)
			                          	{
			                          		return (ValidationResult)method.Invoke(component, null);
			                          	});
		}
	}
}
