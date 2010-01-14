namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Defines an interface to a class that acts as a source of validation rule sets.
	/// </summary>
	public interface IValidationRuleSetSource
	{
		/// <summary>
		/// Gets the validation rule set for the specified class.
		/// </summary>
		/// <param name="class"></param>
		/// <returns></returns>
		ValidationRuleSet GetRuleSet(string @class);
	}
}
