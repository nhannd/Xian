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
