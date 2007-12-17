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
}
