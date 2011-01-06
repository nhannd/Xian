#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using ClearCanvas.Dicom.Iod;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Tests
{
	[TestFixture]
	public class DicomStringHelperTests
	{
		public DicomStringHelperTests()
		{
		}

		[Test]
		public void TestStringArrayConverter()
		{
			string input = null;
			string[] output = DicomStringHelper.GetStringArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = DicomStringHelper.GetStringArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"the\lazy\\brown\dog";
			output = DicomStringHelper.GetStringArray(input);
			Assert.AreEqual(output[0], "the");
			Assert.AreEqual(output[1], "lazy");
			Assert.AreEqual(output[2], "brown");
			Assert.AreEqual(output[3], "dog");
		}

		[Test]
		public void TestDoubleArrayConverter()
		{
			string input = null;
			double[] output;
			DicomStringHelper.TryGetDoubleArray(input, out output);
			Assert.AreEqual(output.Length, 0);

			input = "";
			DicomStringHelper.TryGetDoubleArray(input, out output);
			Assert.AreEqual(output.Length, 0);

			input = @"0\1.2\2.3";
			DicomStringHelper.TryGetDoubleArray(input, out output);
			Assert.AreEqual(output[0], 0);
			Assert.AreEqual(output[1], 1.2);
			Assert.AreEqual(output[2], 2.3);
		}
		
		[Test]
		public void TestIntArrayConverter()
		{
			string input = null;
			int[] output;
			DicomStringHelper.TryGetIntArray(input, out output);
			Assert.AreEqual(output.Length, 0);

			input = "";
			DicomStringHelper.TryGetIntArray(input, out output);
			Assert.AreEqual(output.Length, 0);

			input = @"0\1\30";
			DicomStringHelper.TryGetIntArray(input, out output);
			Assert.AreEqual(output[0], 0);
			Assert.AreEqual(output[1], 1);
			Assert.AreEqual(output[2], 30);
		}

		[Test]
		public void TestPersonNameArrayConverter()
		{
			string input = null;
			PersonName[] output = DicomStringHelper.GetPersonNameArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = DicomStringHelper.GetPersonNameArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"Doe^John^^^\Doe^Jane^^^";
			output = DicomStringHelper.GetPersonNameArray(input);
			Assert.AreEqual(output[0].FirstName, "John");
			Assert.AreEqual(output[0].LastName, "Doe");

			Assert.AreEqual(output[1].FirstName, "Jane");
			Assert.AreEqual(output[1].LastName, "Doe");
		}
	}
}

#endif