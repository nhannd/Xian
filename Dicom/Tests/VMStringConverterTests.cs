#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class VMStringConverterTests
	{
		public VMStringConverterTests()
		{
		}

		[Test]
		public void TestStringArrayConverter()
		{
			string input = null;
			string[] output = VMStringConverter.ToStringArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = VMStringConverter.ToStringArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"the\lazy\\brown\dog";
			output = VMStringConverter.ToStringArray(input);
			Assert.AreEqual(output[0], "the");
			Assert.AreEqual(output[1], "lazy");
			Assert.AreEqual(output[2], "brown");
			Assert.AreEqual(output[3], "dog");
		}

		[Test]
		public void TestDoubleArrayConverter()
		{
			string input = null;
			double[] output = VMStringConverter.ToDoubleArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = VMStringConverter.ToDoubleArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"0\1.2\2.3";
			output = VMStringConverter.ToDoubleArray(input);
			Assert.AreEqual(output[0], 0);
			Assert.AreEqual(output[1], 1.2);
			Assert.AreEqual(output[2], 2.3);
		}
		
		[Test]
		public void TestIntArrayConverter()
		{
			string input = null;
			int[] output = VMStringConverter.ToIntArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = VMStringConverter.ToIntArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"0\1\30";
			output = VMStringConverter.ToIntArray(input);
			Assert.AreEqual(output[0], 0);
			Assert.AreEqual(output[1], 1);
			Assert.AreEqual(output[2], 30);
		}

		[Test]
		public void TestPersonNameArrayConverter()
		{
			string input = null;
			PersonName[] output = VMStringConverter.ToPersonNameArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = VMStringConverter.ToPersonNameArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"Doe^John^^^\Doe^Jane^^^";
			output = VMStringConverter.ToPersonNameArray(input);
			Assert.AreEqual(output[0].FirstName, "John");
			Assert.AreEqual(output[0].LastName, "Doe");

			Assert.AreEqual(output[1].FirstName, "Jane");
			Assert.AreEqual(output[1].LastName, "Doe");
		}
	}
}

#endif