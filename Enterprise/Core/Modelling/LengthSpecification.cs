#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Specifies minimum and maximum allowable length of the value of a given string-typed property of an object.
	/// </summary>
	internal class LengthSpecification : SimpleInvariantSpecification
	{
		private readonly int _min;
		private readonly int _max;

		internal LengthSpecification(PropertyInfo property, int min, int max)
			: base(property)
		{
			_min = min;
			_max = max;
		}

		public override TestResult Test(object obj)
		{
			var value = GetPropertyValue(obj);

			// ignore null values
			if (value == null)
				return new TestResult(true);

			try
			{
				var text = (string)value;

				return text.Length >= _min && text.Length <= _max ? new TestResult(true) :
					new TestResult(false, new TestResultReason(GetMessage()));

			}
			catch (InvalidCastException e)
			{
				throw new SpecificationException(SR.ExceptionExpectedStringValue, e);
			}
		}

		private string GetMessage()
		{
			return string.Format(SR.RuleLength,
				TerminologyTranslator.Translate(this.Property), _min, _max);
		}
	}
}
