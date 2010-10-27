#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Annotations.Dicom.Tests
{

#pragma warning disable 1591,0419,1574,1587

	[TestFixture]
	public class DicomBasicFormatterTests
	{
		public DicomBasicFormatterTests()
		{ }

		[Test]
		public void TestListFormatters()
		{
			string input = @"The\brown\dog\\jumped";
			string result = DicomDataFormatHelper.StringListFormat(DicomStringHelper.GetStringArray(input));
			Assert.AreEqual(result, "The,\nbrown,\ndog,\njumped");

			input = @"Doe^John^^^";
			result = DicomDataFormatHelper.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "Doe, John");

			input = @"^John^^^";
			result = DicomDataFormatHelper.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "John");

			input = @"Doe^^^^";
			result = DicomDataFormatHelper.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "Doe");

			input = @"Doe^John^^^\Doe^Jane^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "Doe, John,\nDoe, Jane");

			input = @"^John^^^\Doe^Jane^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "John,\nDoe, Jane");

			input = @"^John^^^\Doe^^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "John,\nDoe");

			input = @"^^^^\Doe^^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "Doe");

			input = @"^^^^\^^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "");
		}
	}
}

#endif