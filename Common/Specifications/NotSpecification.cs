#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591
namespace ClearCanvas.Common.Specifications
{
	public class NotSpecification : CompositeSpecification
	{
		public NotSpecification()
		{
		}

		protected override TestResult InnerTest(object exp, object root)
		{
			foreach (ISpecification subSpec in this.Elements)
			{
				TestResult r = subSpec.Test(exp);
				if (r.Fail)
					return new TestResult(true);
			}
			return new TestResult(false, new TestResultReason(this.FailureMessage));
			
		}
	}
}
