using System;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	public class ValidationRule<T> : ISpecification
	{
		private readonly Converter<T, TestResult> _func;

		public ValidationRule(Converter<T, TestResult> func)
		{
			_func = func;
		}

		public TestResult Test(object obj)
		{
			return _func((T)obj);
		}
	}
}
