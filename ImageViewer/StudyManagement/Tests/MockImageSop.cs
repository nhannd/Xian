#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	internal interface IMockImageSopSetters
	{
		string StudyInstanceUid { set; }
		string SeriesInstanceUid { set; }
		
		int InstanceNumber { set; }

		int PixelRepresentation { set; }
		int BitsStored { set; }
		Window[] WindowCenterAndWidth { set; }
	}

	// This mock IImageSop only implements the bare minimum of the overridden properties
	// in order to perform unit tests.
	internal class MockImageSop : ImageSop, IMockImageSopSetters
	{
		private int _bitsStored = 16;
		private int _pixelRepresentation = 0;
		private Window[] _windowCentersAndWidths;

		private double _rescaleIntercept = 0;
		private double _rescaleSlope = 1.0;
		private int _rows = 256;
		private int _columns = 256;
		private PixelSpacing _pixelSpacing = new PixelSpacing(1.0, 1.0);
		private PhotometricInterpretation _photometricInterpretation = PhotometricInterpretation.Monochrome1;

		private string _studyInstanceUID;
		private string _seriesInstanceUID;
		private int _instanceNumber;

		public MockImageSop()
		{
		}

        public override object NativeDicomObject
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

		public override int BitsStored
		{
			get { return _bitsStored; }
		}

		public override int PixelRepresentation
		{
			get { return _pixelRepresentation; }
		}

		public override Window[] WindowCenterAndWidth
		{
			get { return _windowCentersAndWidths; }
		}

		public override double RescaleIntercept
		{
			get { return _rescaleIntercept; }
		}

		public override double RescaleSlope
		{
			get { return _rescaleSlope; }
		}

		public override int Rows
		{
			get { return _rows; }
		}

		public override int Columns
		{
			get { return _columns; }
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get { return _photometricInterpretation; }
		}

		public override PixelSpacing PixelSpacing
		{
			get { return _pixelSpacing; }
		}

		public override string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
		}

		public override string SeriesInstanceUID
		{
			get { return _seriesInstanceUID; }
		}

		public override int InstanceNumber
		{
			get { return _instanceNumber; }
		}

		#region IMockImageSopSetters Members

		string IMockImageSopSetters.StudyInstanceUid
		{
			set { _studyInstanceUID = value; }
		}

		string IMockImageSopSetters.SeriesInstanceUid
		{
			set { _seriesInstanceUID = value; }
		}

		int IMockImageSopSetters.InstanceNumber
		{
			set { _instanceNumber = value; }
		}

		int IMockImageSopSetters.PixelRepresentation
		{ 
			set { _pixelRepresentation = value; } 
		}

		int IMockImageSopSetters.BitsStored
		{
			set { _bitsStored = value; }
		}

		Window[] IMockImageSopSetters.WindowCenterAndWidth
		{
			set { _windowCentersAndWidths = value; }
		}

		#endregion

		#region Not Implemented

		public override PersonName PatientsName
		{
			get
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
		}

		public override string PatientsBirthDate
		{
			get
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
		}

		public override string StudyDate
		{
			get
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
		}

		public override PersonName ReferringPhysiciansName
		{
			get
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
		}

		public override string StudyDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string[] AdmittingDiagnosesDescription
		{
			get
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
		}

		public override string AdditionalPatientsHistory
		{
			get
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
		}

		public override int SeriesNumber
		{
			get
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
		}

		public override string Laterality
		{
			get
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
		}

		public override string SeriesTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] PerformingPhysiciansName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ProtocolName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] OperatorsName
		{
			get
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
		}

		public override string PatientPosition
		{
			get
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
		}

		public override string InstitutionName
		{
			get
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
		}

		public override string InstitutionalDepartmentName
		{
			get
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
		}

		public override PatientOrientation PatientOrientation
		{
			get
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
		}

		public override int AcquisitionNumber
		{
			get
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
		}

		public override string AcquisitionTime
		{
			get
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
		}

		public override int ImagesInAcquisition
		{
			get
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
		}

		public override string LossyImageCompression
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double[] LossyImageCompressionRatio
		{
			get
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
		}


		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override ImagePositionPatient ImagePositionPatient
		{
			get
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
		}

		public override double SliceLocation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
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
		}

		public override int BitsAllocated
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override byte[] PixelData
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int PlanarConfiguration
		{
			get
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
		}

		public override string[] WindowCenterAndWidthExplanation
		{
			get
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

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out int val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out int val, uint position, out bool tagExists)
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

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out string val, uint pos, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTagArray(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out string valueArray, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string SopInstanceUID
		{
			get
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
		}

		public override int HighBit
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion
	}
}
#endif