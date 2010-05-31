using System;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Encapsulates a validation rule expressed as a function.
	/// </summary>
	/// <typeparam name="T">The class to which the rule applies.</typeparam>
	public class ValidationRule<T> : ISpecification
	{
		private readonly Converter<T, TestResult> _func;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="func">A function that expresses the validation rule.</param>
		public ValidationRule(Converter<T, TestResult> func)
		{
			_func = func;
		}

		/// <summary>
		/// Tests this rule against the specified instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public TestResult Test(T obj)
		{
			return _func(obj);
		}

		TestResult ISpecification.Test(object obj)
		{
			return Test((T)obj);
		}
	}
}
