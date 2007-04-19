#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.ImageViewer.AnnotationProviders.Dicom;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Tests
{
	[TestFixture]
	public class DicomBasicFormatterTests
	{
		public DicomBasicFormatterTests()
		{ }

		[Test]
		public void TestListFormatters()
		{
			string input = @"The\brown\dog\\jumped";
			string result = DicomBasicResultFormatter.StringListFormat(VMStringConverter.ToStringArray(input));
			Assert.AreEqual(result, "The,\nbrown,\ndog,\njumped");

			input = @"Doe^John^^^";
			result = DicomBasicResultFormatter.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "Doe, John");

			input = @"^John^^^";
			result = DicomBasicResultFormatter.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "John");

			input = @"Doe^^^^";
			result = DicomBasicResultFormatter.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "Doe");

			input = @"Doe^John^^^\Doe^Jane^^^";
			result = DicomBasicResultFormatter.PersonNameListFormatter(VMStringConverter.ToPersonNameArray(input));
			Assert.AreEqual(result, "Doe, John,\nDoe, Jane");

			input = @"^John^^^\Doe^Jane^^^";
			result = DicomBasicResultFormatter.PersonNameListFormatter(VMStringConverter.ToPersonNameArray(input));
			Assert.AreEqual(result, "John,\nDoe, Jane");

			input = @"^John^^^\Doe^^^^";
			result = DicomBasicResultFormatter.PersonNameListFormatter(VMStringConverter.ToPersonNameArray(input));
			Assert.AreEqual(result, "John,\nDoe");

			input = @"^^^^\Doe^^^^";
			result = DicomBasicResultFormatter.PersonNameListFormatter(VMStringConverter.ToPersonNameArray(input));
			Assert.AreEqual(result, "Doe");

			input = @"^^^^\^^^^";
			result = DicomBasicResultFormatter.PersonNameListFormatter(VMStringConverter.ToPersonNameArray(input));
			Assert.AreEqual(result, "");
		}
	}
}

#endif