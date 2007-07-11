using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tests
{
	class TestImageSop : ImageSop
	{
		private string _patientID;
		private string _studyInstanceUID;
		private string _seriesInstanceUID;
		private string _sopInstanceUID;

		public TestImageSop(
			string patientID, 
			string studyInstanceUID, 
			string seriesInstanceUID, 
			string sopInstanceUID)
		{
			_patientID = patientID;
			_studyInstanceUID = studyInstanceUID;
			_seriesInstanceUID = seriesInstanceUID;
			_sopInstanceUID = sopInstanceUID;
		}

		public override object NativeDicomObject
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

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
				return _patientID;
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

		public override string StudyInstanceUID
		{
			get
			{
				return _studyInstanceUID;
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

		public override string SeriesInstanceUID
		{
			get
			{
				return _seriesInstanceUID;
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

		public override int InstanceNumber
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

		public override PixelSpacing PixelSpacing
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
				return 1;
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return PhotometricInterpretation.Monochrome1;
			}
		}

		public override int Rows
		{
			get
			{
				return 512;
			}
		}

		public override int Columns
		{
			get
			{
				return 512;
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return 16;
			}
		}

		public override int BitsStored
		{
			get
			{
				return 16;
			}
		}

		public override int HighBit
		{
			get
			{
				return 15;
			}
		}

		public override int PixelRepresentation
		{
			get
			{
				return 0;
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

		public override double RescaleIntercept
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleSlope
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

		public override Window[] WindowCenterAndWidth
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

		public override void GetTag(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out string val, uint position, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTagArray(ClearCanvas.Dicom.OffisWrapper.DcmTagKey tag, out string val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string SopInstanceUID
		{
			get
			{
				return _sopInstanceUID;
			}
		}

		public override string TransferSyntaxUID
		{
			get
			{
				return "1.2.840.10008.1.2";
			}
		}
	}
}
