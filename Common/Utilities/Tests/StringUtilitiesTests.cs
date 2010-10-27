#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	public class StringUtilitiesTests
	{
		public StringUtilitiesTests()
		{ 
		}

		[Test]
		public void TestDoubleCombine()
		{
			string expectedResult = "2.22, 2.33";
			string result = StringUtilities.Combine<double>(new double[] { 2.2222, 2.333 }, ", ", "F2");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "2.22, 2.33, NaN";
			result = StringUtilities.Combine<double>(new double[] { 2.2222, 2.333, double.NaN }, ", ", "F2");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "2.22, 2.33, Infinity";
			result = StringUtilities.Combine<double>(new double[] { 2.2222, 2.333, double.PositiveInfinity}, ", ", "F2");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "2.22, 2.33, -Infinity";
			result = StringUtilities.Combine<double>(new double[] { 2.2222, 2.333, double.NegativeInfinity}, ", ", "F2");
			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void TestStringCombine()
		{
			#region Values/Separator Combine function

			string expectedResult = "";
			string result = StringUtilities.Combine<string>(new string[] { }, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] { "" }, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "MR, CT, DX";
			result = StringUtilities.Combine<string>(new string[] { "MR", "CT", "DX" }, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "MR, DX";
			result = StringUtilities.Combine<string>(new string[] { "MR", "", "DX" }, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "MR";
			result = StringUtilities.Combine<string>(new string[] { null, "MR", "" }, ", ");
			Assert.AreEqual(expectedResult, result);

			#endregion

			#region Values/Separator/SkipEmpty=false Combine Function
			
			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] { }, ", ", false);
			Assert.AreEqual(expectedResult, result);
			
			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] { "" }, ", ", false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = ", ";
			result = StringUtilities.Combine<string>(new string[] { null, "" }, ", ", false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = ", MR, ";
			result = StringUtilities.Combine<string>(new string[] { "", "MR", null }, ", ", false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = ", , MR, ";
			result = StringUtilities.Combine<string>(new string[] { "", null, "MR", "" }, ", ", false);
			Assert.AreEqual(expectedResult, result);

			#endregion

			#region Values/Separator/FormattingDelegate Combine Function

			StringUtilities.FormatDelegate<string> noNullCheckFormattingDelegate = delegate(string value) { return String.Format("'{0}'", value); };

			expectedResult = "'MR', 'CT', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { "MR", "CT", "DX" }, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { "MR", "", "DX" }, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', 'CT', ''";
			result = StringUtilities.Combine<string>(new string[] { "MR", "CT", "" }, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', 'CT', ''";
			result = StringUtilities.Combine<string>(new string[] { null, "CT", "" }, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { null, "", "DX" }, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', ''";
			result = StringUtilities.Combine<string>(new string[] { null, "", "" }, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			//now change the delegate

			StringUtilities.FormatDelegate<string> nullCheckFormattingDelegate = delegate(string value) { return String.IsNullOrEmpty(value) ? "" : String.Format("'{0}'", value); };

			expectedResult = "'MR', 'CT', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { "MR", "CT", "DX" }, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { "MR", "", "DX" }, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', 'CT'";
			result = StringUtilities.Combine<string>(new string[] { "MR", "CT", "" }, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'CT'";
			result = StringUtilities.Combine<string>(new string[] { null, "CT", "" }, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'DX'";
			result = StringUtilities.Combine<string>(new string[] { null, "", "DX" }, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] { null, "", "" }, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			#endregion

			#region Values/Separator/FormattingDelegate/SkipEmpty=false Combine Function

			expectedResult = "'MR', 'CT', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { "MR", "CT", "DX" }, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { "MR", "", "DX" }, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] { "", "", "DX" }, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', ''";
			result = StringUtilities.Combine<string>(new string[] { "", "", null }, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			#endregion	
		}
	}
}

#endif