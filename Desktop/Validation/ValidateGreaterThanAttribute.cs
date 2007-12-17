
namespace ClearCanvas.Desktop.Validation
{
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
			: base(referenceProperty)
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
}
