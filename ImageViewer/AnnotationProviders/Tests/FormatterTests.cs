#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Tests
{
	[TestFixture]
	public class FormatterTests
	{
		public FormatterTests()
		{ 
		}

		[Test]
		public void TestDoubleFormatter()
		{
			DoubleFormatter formatter = new DoubleFormatter();
			double input = 1.2345;
			string result = formatter.Format(input);
			Assert.AreEqual(result, "1.23");

			double[] inputs = new double[] { 1.2345, 10.6789 };

			result = formatter.FormatList(inputs);
			Assert.AreEqual(result, "1.23,\n10.68");

			inputs = new double[] { };

			result = formatter.FormatList(inputs);
			Assert.AreEqual(result, "");
		}
	}
}

#endif