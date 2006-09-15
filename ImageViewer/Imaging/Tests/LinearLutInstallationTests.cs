using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	// This mock ImageSop is very specific to the Linear Lut Installation tests
	// as it only implements the bare minimum of the overridden properties.
	public class LinearLutInstallationTestImageSop : ImageSop
	{
		private int _bitsStored;
		private int _pixelRepresentation;
		private double _windowWidth;
		private double _windowCenter;

		public LinearLutInstallationTestImageSop(
			int pixelRepresentation,
			int bitsStored,
			double windowWidth,
			double windowCenter)
		{
			_pixelRepresentation = pixelRepresentation;
			_bitsStored = bitsStored;
			_windowWidth = windowWidth;
			_windowCenter = windowCenter;
		}

		public override int BitsStored
		{
			get { return _bitsStored; }
			set { _bitsStored = value; }
		}

		public override int PixelRepresentation
		{
			get { return _pixelRepresentation; }
			set { _pixelRepresentation = value; }
		}

		public override double WindowCenter
		{
			get { return _windowCenter; }
			set { _windowCenter = value; }
		}

		public override double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		public override double RescaleIntercept
		{
			get
			{
				return 0;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleSlope
		{
			get
			{
				return 1;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int Rows
		{
			get
			{
				return 256;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int Columns
		{
			get
			{
				return 256;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelSpacingX
		{
			get
			{
				return 0;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelSpacingY
		{
			get
			{
				return 0;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PhotometricInterpretation
		{
			get { return "MONOCHROME1"; }
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#region Not Implemented
		public override string PatientsName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientId
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsBirthDate
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsSex
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyInstanceUID
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyDate
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ReferringPhysiciansName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AccessionNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string NameOfPhysiciansReadingStudy
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AdmittingDiagnosesDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsAge
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AdditionalPatientsHistory
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Modality
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesInstanceUID
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Laterality
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesDate
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PerformingPhysiciansName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string OperatorsName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string BodyPartExamined
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientPosition
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Manufacturer
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstitutionName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StationName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstitutionalDepartmentName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ManufacturersModelName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstanceNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientOrientationRows
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientOrientationColumns
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImageType
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionDate
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionDateTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImagesInAcquisition
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImageComments
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string LossyImageCompression
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string LossyImageCompressionRatio
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PresentationLUTShape
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientRowX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientRowY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientRowZ
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientColumnX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientColumnY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientColumnZ
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImagePositionPatientX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImagePositionPatientY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImagePositionPatientZ
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double SliceThickness
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double SliceLocation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelAspectRatioX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelAspectRatioY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int SamplesPerPixel
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int BitsAllocated
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override byte[] GetPixelData()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override int PlanarConfiguration
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string RescaleType
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double WindowCenterAndWidthExplanation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out ushort val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out ushort val, uint position, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out double val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out double val, uint position, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out string val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string SopInstanceUID
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string TransferSyntaxUID
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int HighBit
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion
	}

	[TestFixture]
	public class LinearLutInstallationTests
	{
		public LinearLutInstallationTests()
		{
		}
		
		[TestFixtureSetUp]
		public void Init()
		{
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void TestNoWindowLevelSpecified()
		{
			TestSingleConfiguration(0, 7, double.NaN, double.NaN, 128, 64);
			TestSingleConfiguration(0, 8, double.NaN, double.NaN, 256, 128);
			TestSingleConfiguration(0, 10, double.NaN, double.NaN, 1024, 512);
			TestSingleConfiguration(0, 11, double.NaN, double.NaN, 2048, 1024);
			TestSingleConfiguration(0, 12, double.NaN, double.NaN, 4096, 2048);
			TestSingleConfiguration(0, 13, double.NaN, double.NaN, 8192, 4096);
			TestSingleConfiguration(0, 14, double.NaN, double.NaN, 16384, 8192);
			TestSingleConfiguration(0, 15, double.NaN, double.NaN, 32768, 16384);
			TestSingleConfiguration(0, 16, double.NaN, double.NaN, 65536, 32768);

			TestSingleConfiguration(1, 7, double.NaN, double.NaN, 128, 0);
			TestSingleConfiguration(1, 8, double.NaN, double.NaN, 256, 0);
			TestSingleConfiguration(1, 10, double.NaN, double.NaN, 1024, 0);
			TestSingleConfiguration(1, 11, double.NaN, double.NaN, 2048, 0);
			TestSingleConfiguration(1, 12, double.NaN, double.NaN, 4096, 0);
			TestSingleConfiguration(1, 13, double.NaN, double.NaN, 8192, 0);
			TestSingleConfiguration(1, 14, double.NaN, double.NaN, 16384, 0);
			TestSingleConfiguration(1, 15, double.NaN, double.NaN, 32768, 0);
			TestSingleConfiguration(1, 16, double.NaN, double.NaN, 65536, 0);
		}

		[Test]
		public void TestOnlyWindowSpecified()
		{
			TestSingleConfiguration(1, 10, 411, double.NaN, 411, 0);
			TestSingleConfiguration(0, 10, 411, double.NaN, 411, 205);
		}

		[Test]
		public void TestOnlyLevelSpecified()
		{
			TestSingleConfiguration(1, 10, double.NaN, 410, 1024, 410); 
			TestSingleConfiguration(0, 10, double.NaN, 410, 1024, 410);
		}

		[Test]
		public void TestWindowLevelSpecified()
		{
			TestSingleConfiguration(1, 10, 411, 205, 411, 205);
		}

		public void TestSingleConfiguration(
			int pixelRepresentation,
			int bitsStored,
			double windowWidth,
			double windowCenter,
			double expectedWindowWidth,
			double expectedWindowCenter)
		{
			LinearLutInstallationTestImageSop imageSop = new LinearLutInstallationTestImageSop(pixelRepresentation, bitsStored, windowWidth, windowCenter);
			DicomPresentationImage image = new DicomPresentationImage(imageSop);

			WindowLevelOperator.InstallVOILUTLinear(image);
			VOILUTLinear lut = image.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline.VoiLUT as VOILUTLinear;
			
			Assert.AreEqual(lut.WindowWidth, expectedWindowWidth);
			Assert.AreEqual(lut.WindowCenter, expectedWindowCenter);
		}
	}
}
