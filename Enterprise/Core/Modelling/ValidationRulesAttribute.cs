using System;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ValidationRulesAttribute : Attribute
	{
		public ValidationRulesAttribute(string methodName)
		{
			this.MethodName = methodName;
		}

		internal string MethodName { get; private set; }
	}
}
