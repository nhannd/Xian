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

		/// <summary>
		/// Verifies the method signature.
		/// </summary>
		protected void CheckMethodSignature(MethodInfo info)
		{
			if (!(typeof(ValidationResult).IsAssignableFrom(info.ReturnType)))
				throw new ValidationAttributeException("The decorated method does not have the correct signature.");

			ParameterInfo[] parameters = info.GetParameters();
			if (parameters.Length != 0)
				throw new ValidationAttributeException("The decorated method does not have the correct signature.");
		}

		/// <summary>
		/// Factory method that creates an <see cref="IValidationRule"/> for 
		/// the property with the name <see cref="PropertyName"/>.
		/// </summary>
		public IValidationRule CreateRule(MethodInfo method)
		{
			return new ValidationRule(_propertyName,
			                          delegate(IApplicationComponent component)
			                          	{
			                          		return (ValidationResult)method.Invoke(component, null);
			                          	});
		}
	}
}
