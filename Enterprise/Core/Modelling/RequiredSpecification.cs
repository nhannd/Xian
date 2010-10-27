#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Reflection;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Specifies that a given property of an object is required to have a value (e.g. non-null).
	/// </summary>
	internal class RequiredSpecification : SimpleInvariantSpecification
	{
		internal RequiredSpecification(PropertyInfo property)
			: base(property)
		{
		}

		public override TestResult Test(object obj)
		{
			var value = GetPropertyValue(obj);

			// special consideration for strings - empty string should be considered "null"
			var isNull = (value is string) ? string.IsNullOrEmpty((string)value) : value == null;

			return isNull ? new TestResult(false, new TestResultReason(GetMessage())) : new TestResult(true);
		}

		private string GetMessage()
		{
			return string.Format(SR.RuleRequired, TerminologyTranslator.Translate(this.Property));
		}
	}
}
