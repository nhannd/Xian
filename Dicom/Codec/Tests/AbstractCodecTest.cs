#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Codec.Tests
{
	[TestFixture]
	public class CodecTest : AbstractCodecTest
	{
		[Test]
		public void Test()
		{
			DicomFile file = CreateFile(256, 256, "MONOCHROME1", 12, 16, true, 1);
			file.Filename = "Monochrome1TestPattern.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
            file.Save();

			file = CreateFile(256, 256, "MONOCHROME2", 14, 16, true, 1);
			file.Filename = "Monochrome2TestPattern.dcm";
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
            file.Save();

			file = CreateFile(256, 256, "RGB", 8, 8, false, 1);
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
			file.Filename = "RgbColorTestPattern.dcm";
			file.Save();

			file = CreateFile(256, 256, "YBR_FULL", 8, 8, false, 1);
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
			file.Filename = "YbrColorTestPattern.dcm";
			file.Save();

		}
	}
	public class AbstractCodecTest : AbstractTest
	{
        public void SetupMRWithUNVR(DicomAttributeCollection theSet)
        {
            SetupMR(theSet);

            theSet[DicomTags.LossyImageCompressionMethod].SetStringValue("ISO_15444_1");

            ConvertAttributeToUN(theSet, DicomTags.LossyImageCompressionMethod);
        }

        public void SetupMRWithOverlay(DicomAttributeCollection theSet)
        {
            SetupMR(theSet);

            OverlayPlaneModuleIod overlayIod = new OverlayPlaneModuleIod(theSet);
            DicomUncompressedPixelData pd = new DicomUncompressedPixelData(theSet);
            OverlayPlane overlay = overlayIod[0];

            // Embedded overlays are retired in dicom, just doing it for testing purposes
            theSet[DicomTags.OverlayBitPosition].SetInt32(0, pd.HighBit + 1);
            overlay.OverlayBitsAllocated = 1;
            overlay.OverlayColumns = pd.ImageWidth;
            overlay.OverlayRows = pd.ImageHeight;
            overlay.OverlayOrigin = new Point(0, 0);
            overlay.OverlayType = OverlayType.R;
        }

	    public static void LosslessImageTest(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

			List<DicomAttributeComparisonResult> list = new List<DicomAttributeComparisonResult>();
			bool result = newFile.DataSet.Equals(saveCopy.DataSet, ref list);

			StringBuilder sb = new StringBuilder();
			foreach (DicomAttributeComparisonResult compareResult in list)
				sb.AppendFormat("Comparison Failure: {0}, ", compareResult.Details);

			Assert.IsTrue(result,sb.ToString());
		}

		public static void LosslessImageTestWithConversion(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

			Assert.IsFalse(newFile.DataSet.Equals(saveCopy.DataSet));
		}

		public static void LossyImageTest(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

			Assert.IsFalse(newFile.DataSet.Equals(saveCopy.DataSet));

			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.DerivationDescription));
			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.LossyImageCompression));
			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.LossyImageCompressionMethod));
			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.LossyImageCompressionRatio));

			Assert.IsFalse(newFile.DataSet[DicomTags.DerivationDescription].IsEmpty);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompression].IsEmpty);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionMethod].IsEmpty);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionRatio].IsEmpty);

			Assert.IsFalse(newFile.DataSet[DicomTags.DerivationDescription].IsNull);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompression].IsNull);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionMethod].IsNull);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionRatio].IsNull);

			// Make copies of datasets, delete the tags that don't match, and ensure the same
			DicomAttributeCollection newDataSet = newFile.DataSet.Copy(true, true, true);
			DicomAttributeCollection oldDataSet = theFile.DataSet.Copy(true, true, true);

			oldDataSet.RemoveAttribute(DicomTags.PixelData);
			newDataSet.RemoveAttribute(DicomTags.PixelData);
			oldDataSet.RemoveAttribute(DicomTags.DerivationDescription);
			newDataSet.RemoveAttribute(DicomTags.DerivationDescription);
			oldDataSet.RemoveAttribute(DicomTags.LossyImageCompression);
			newDataSet.RemoveAttribute(DicomTags.LossyImageCompression);
			oldDataSet.RemoveAttribute(DicomTags.LossyImageCompressionMethod);
			newDataSet.RemoveAttribute(DicomTags.LossyImageCompressionMethod);
			oldDataSet.RemoveAttribute(DicomTags.LossyImageCompressionRatio);
			newDataSet.RemoveAttribute(DicomTags.LossyImageCompressionRatio);
			oldDataSet.RemoveAttribute(DicomTags.PhotometricInterpretation);
			newDataSet.RemoveAttribute(DicomTags.PhotometricInterpretation);

			List<DicomAttributeComparisonResult> results = new List<DicomAttributeComparisonResult>();

			bool check = oldDataSet.Equals(newDataSet, ref results);
			Assert.IsTrue(check, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);
		}

		public static void ExpectedFailureTest(TransferSyntax syntax, DicomFile theFile)
		{
			try
			{
				theFile.ChangeTransferSyntax(syntax);
			}
			catch (DicomCodecUnsupportedSopException)
			{
				return;
			}

			Assert.IsTrue(false, "Unexpected successfull compression of object.");
		}
	}
}
