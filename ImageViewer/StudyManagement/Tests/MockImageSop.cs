using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	// This mock ImageSop only implements the bare minimum of the overridden properties
	// in order to perform units tests.
	public class MockImageSop : ImageSop
	{
		private int _bitsStored = 16;
		private int _pixelRepresentation = 0;
		private double _windowWidth = 1;
		private double _windowCenter = 0;

		private double _rescaleIntercept = 0;
		private double _rescaleSlope = 1.0;
		private int _rows = 256;
		private int _columns = 256;
		private double _pixelSpacingX = 0;
		private double _pixelSpacingY = 0;
		private string _photometricInterpretation = "MONOCHROME1";

		private string _studyInstanceUID;
		private string _seriesInstanceUID;
		private string _instanceNumber;

		public MockImageSop()
		{
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
			get { return _rescaleIntercept; }
			set { _rescaleIntercept = value;  }
		}

		public override double RescaleSlope
		{
			get { return _rescaleSlope; }
			set { _rescaleSlope = value; }
		}

		public override int Rows
		{
			get { return _rows; }
			set { _rows = value; }
		}

		public override int Columns
		{
			get { return _columns; }
			set { _columns = value; }
		}

		public override double PixelSpacingX
		{
			get { return _pixelSpacingX; }
			set { _pixelSpacingX = value; }
		}

		public override double PixelSpacingY
		{
			get { return _pixelSpacingY; }
			set { _pixelSpacingY = value; }
		}

		public override string PhotometricInterpretation
		{
			get { return _photometricInterpretation; }
			set { _photometricInterpretation = value; }
		}

		public override string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
			set { _studyInstanceUID = value; }
		}

		public override string SeriesInstanceUID
		{
			get { return _seriesInstanceUID; }
			set { _seriesInstanceUID = value; }
		}

		public override string InstanceNumber
		{
			get { return _instanceNumber; }
			set { _instanceNumber = value; }
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
}
