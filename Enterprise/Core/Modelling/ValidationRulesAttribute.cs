using System;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// When applied to an entity class, specifies the name of a method that supplies a validation rule set
	/// for the class.
	/// </summary>
	/// <remarks>
	/// The method must be static, and have the signature
	/// <code>
	/// IValidationRuleSet MyMethod()
	/// </code>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ValidationRulesAttribute : Attribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="methodName"></param>
		public ValidationRulesAttribute(string methodName)
		{
			this.MethodName = methodName;
		}

		/// <summary>
		/// Gets the name of the method that supplies the validation rule set.
		/// </summary>
		internal string MethodName { get; private set; }
	}
}
