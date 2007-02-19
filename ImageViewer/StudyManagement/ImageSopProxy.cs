using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class ImageSopProxy : ImageSop
	{
		private ImageSop _realImageSop;

		internal ImageSopProxy(ImageSop realImageSop)
		{
			Platform.CheckForNullReference(realImageSop, "realImageSop");
			_realImageSop = realImageSop;
		}

		public override object NativeDicomObject
		{
			get 
			{ 
				return _realImageSop.NativeDicomObject; 
			}
		}

		public override PersonName PatientsName
		{
			get
			{
				return _realImageSop.PatientsName;
			}
			set
			{
				_realImageSop.PatientsName = value;
			}
		}

		public override string PatientId
		{
			get
			{
				return _realImageSop.PatientId;
			}
			set
			{
				_realImageSop.PatientId = value;
			}
		}

		public override string PatientsBirthDate
		{
			get
			{
				return _realImageSop.PatientsBirthDate;
			}
			set
			{
				_realImageSop.PatientsBirthDate = value;
			}
		}

		public override string PatientsSex
		{
			get
			{
				return _realImageSop.PatientsSex;
			}
			set
			{
				_realImageSop.PatientsSex = value;
			}
		}

		public override string StudyInstanceUID
		{
			get
			{
				return _realImageSop.StudyInstanceUID;
			}
			set
			{
				_realImageSop.StudyInstanceUID = value;
			}
		}

		public override string StudyDate
		{
			get
			{
				return _realImageSop.StudyDate;
			}
			set
			{
				_realImageSop.StudyDate = value;
			}
		}

		public override string StudyTime
		{
			get
			{
				return _realImageSop.StudyTime;
			}
			set
			{
				_realImageSop.StudyTime = value;
			}
		}

		public override PersonName ReferringPhysiciansName
		{
			get
			{
				return _realImageSop.ReferringPhysiciansName;
			}
			set
			{
				_realImageSop.ReferringPhysiciansName = value;
			}
		}

		public override string AccessionNumber
		{
			get
			{
				return _realImageSop.AccessionNumber;
			}
			set
			{
				_realImageSop.AccessionNumber = value;
			}
		}

		public override string StudyDescription
		{
			get
			{
				return _realImageSop.StudyDescription;
			}
			set
			{
				_realImageSop.StudyDescription = value;
			}
		}

		public override PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				return _realImageSop.NameOfPhysiciansReadingStudy;
			}
			set
			{
				_realImageSop.NameOfPhysiciansReadingStudy = value;
			}
		}

		public override string[] AdmittingDiagnosesDescription
		{
			get
			{
				return _realImageSop.AdmittingDiagnosesDescription;
			}
			set
			{
				_realImageSop.AdmittingDiagnosesDescription = value;
			}
		}

		public override string PatientsAge
		{
			get
			{
				return _realImageSop.PatientsAge;
			}
			set
			{
				_realImageSop.PatientsAge = value;
			}
		}

		public override string AdditionalPatientsHistory
		{
			get
			{
				return _realImageSop.AdditionalPatientsHistory;
			}
			set
			{
				_realImageSop.AdditionalPatientsHistory = value;
			}
		}

		public override string Modality
		{
			get
			{
				return _realImageSop.Modality;
			}
			set
			{
				_realImageSop.Modality = value;
			}
		}

		public override string SeriesInstanceUID
		{
			get
			{
				return _realImageSop.SeriesInstanceUID;
			}
			set
			{
				_realImageSop.SeriesInstanceUID = value;
			}
		}

		public override int SeriesNumber
		{
			get
			{
				return _realImageSop.SeriesNumber;
			}
			set
			{
				_realImageSop.SeriesNumber = value;
			}
		}

		public override string SeriesDescription
		{
			get
			{
				return _realImageSop.SeriesDescription;
			}
			set
			{
				_realImageSop.SeriesDescription = value;
			}
		}

		public override string Laterality
		{
			get
			{
				return _realImageSop.Laterality;
			}
			set
			{
				_realImageSop.Laterality = value;
			}
		}

		public override string SeriesDate
		{
			get
			{
				return _realImageSop.SeriesDate;
			}
			set
			{
				_realImageSop.SeriesDate = value;
			}
		}

		public override string SeriesTime
		{
			get
			{
				return _realImageSop.SeriesTime;
			}
			set
			{
				_realImageSop.SeriesTime = value;
			}
		}

		public override PersonName[] PerformingPhysiciansName
		{
			get
			{
				return _realImageSop.PerformingPhysiciansName;
			}
			set
			{
				_realImageSop.PerformingPhysiciansName = value;
			}
		}

		public override PersonName[] OperatorsName
		{
			get
			{
				return _realImageSop.OperatorsName;
			}
			set
			{
				_realImageSop.OperatorsName = value;
			}
		}

		public override string BodyPartExamined
		{
			get
			{
				return _realImageSop.BodyPartExamined;
			}
			set
			{
				_realImageSop.BodyPartExamined = value;
			}
		}

		public override string PatientPosition
		{
			get
			{
				return _realImageSop.PatientPosition;
			}
			set
			{
				_realImageSop.PatientPosition = value;
			}
		}

		public override string Manufacturer
		{
			get
			{
				return _realImageSop.Manufacturer;
			}
			set
			{
				_realImageSop.Manufacturer = value;
			}
		}

		public override string InstitutionName
		{
			get
			{
				return _realImageSop.InstitutionName;
			}
			set
			{
				_realImageSop.InstitutionName = value;
			}
		}

		public override string StationName
		{
			get
			{
				return _realImageSop.StationName;
			}
			set
			{
				_realImageSop.StationName = value;
			}
		}

		public override string InstitutionalDepartmentName
		{
			get
			{
				return _realImageSop.InstitutionalDepartmentName;
			}
			set
			{
				_realImageSop.InstitutionalDepartmentName = value;
			}
		}

		public override string ManufacturersModelName
		{
			get
			{
				return _realImageSop.ManufacturersModelName;
			}
			set
			{
				_realImageSop.ManufacturersModelName = value;
			}
		}

		public override int InstanceNumber
		{
			get
			{
				return _realImageSop.InstanceNumber;
			}
			set
			{
				_realImageSop.InstanceNumber = value;
			}
		}

		public override PatientOrientation PatientOrientation
		{
			get
			{
				return _realImageSop.PatientOrientation;
			}
			set
			{
				_realImageSop.PatientOrientation = value;
			}
		}

		public override string ImageType
		{
			get
			{
				return _realImageSop.ImageType;
			}
			set
			{
				_realImageSop.ImageType = value;
			}
		}

		public override int AcquisitionNumber
		{
			get
			{
				return _realImageSop.AcquisitionNumber;
			}
			set
			{
				_realImageSop.AcquisitionNumber = value;
			}
		}

		public override string AcquisitionDate
		{
			get
			{
				return _realImageSop.AcquisitionDate;
			}
			set
			{
				_realImageSop.AcquisitionDate = value;
			}
		}

		public override string AcquisitionTime
		{
			get
			{
				return _realImageSop.AcquisitionTime;
			}
			set
			{
				_realImageSop.AcquisitionTime = value;
			}
		}

		public override string AcquisitionDateTime
		{
			get
			{
				return _realImageSop.AcquisitionDateTime;
			}
			set
			{
				_realImageSop.AcquisitionDateTime = value;
			}
		}

		public override int ImagesInAcquisition
		{
			get
			{
				return _realImageSop.ImagesInAcquisition;
			}
			set
			{
				_realImageSop.ImagesInAcquisition = value;
			}
		}

		public override string ImageComments
		{
			get
			{
				return _realImageSop.ImageComments;
			}
			set
			{
				_realImageSop.ImageComments = value;
			}
		}

		public override string LossyImageCompression
		{
			get
			{
				return _realImageSop.LossyImageCompression;
			}
			set
			{
				_realImageSop.LossyImageCompression = value;
			}
		}

		public override double[] LossyImageCompressionRatio
		{
			get
			{
				return _realImageSop.LossyImageCompressionRatio;
			}
			set
			{
				_realImageSop.LossyImageCompressionRatio = value;
			}
		}

		public override string PresentationLUTShape
		{
			get
			{
				return _realImageSop.PresentationLUTShape;
			}
			set
			{
				_realImageSop.PresentationLUTShape = value;
			}
		}

		public override PixelSpacing PixelSpacing
		{
			get
			{
				return _realImageSop.PixelSpacing;
			}
			set
			{
				_realImageSop.PixelSpacing = value;
			}
		}

		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				return _realImageSop.ImageOrientationPatient;
			}
			set
			{
				_realImageSop.ImageOrientationPatient = value;
			}
		}

		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				return _realImageSop.ImagePositionPatient;
			}
			set
			{
				_realImageSop.ImagePositionPatient = value;
			}
		}

		public override double SliceThickness
		{
			get
			{
				return _realImageSop.SliceThickness;
			}
			set
			{
				_realImageSop.SliceThickness = value;
			}
		}

		public override double SliceLocation
		{
			get
			{
				return _realImageSop.SliceLocation;
			}
			set
			{
				_realImageSop.SliceLocation = value;
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				return _realImageSop.PixelAspectRatio;
			}
			set
			{
				_realImageSop.PixelAspectRatio = value;
			}
		}

		public override int SamplesPerPixel
		{
			get
			{
				return _realImageSop.SamplesPerPixel;
			}
			set
			{
				_realImageSop.SamplesPerPixel = value;
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return _realImageSop.PhotometricInterpretation;
			}
			set
			{
				_realImageSop.PhotometricInterpretation = value;
			}
		}

		public override int Rows
		{
			get
			{
				return _realImageSop.Rows;
			}
			set
			{
				_realImageSop.Rows = value;
			}
		}

		public override int Columns
		{
			get
			{
				return _realImageSop.Columns;
			}
			set
			{
				_realImageSop.Columns = value;
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return _realImageSop.BitsAllocated;
			}
			set
			{
				_realImageSop.BitsAllocated = value;
			}
		}

		public override int BitsStored
		{
			get
			{
				return _realImageSop.BitsStored;
			}
			set
			{
				_realImageSop.BitsStored = value;
			}
		}

		public override int HighBit
		{
			get
			{
				return _realImageSop.HighBit;
			}
			set
			{
				_realImageSop.HighBit = value;
			}
		}

		public override int PixelRepresentation
		{
			get
			{
				return _realImageSop.PixelRepresentation;
			}
			set
			{
				_realImageSop.PixelRepresentation = value;
			}
		}

		public override byte[] PixelData
		{
			get
			{
				return _realImageSop.PixelData;
			}
		}

		public override int PlanarConfiguration
		{
			get
			{
				return _realImageSop.PlanarConfiguration;
			}
			set
			{
				_realImageSop.PlanarConfiguration = value;
			}
		}

		public override double RescaleIntercept
		{
			get
			{
				return _realImageSop.RescaleIntercept;
			}
			set
			{
				_realImageSop.RescaleIntercept = value;
			}
		}

		public override double RescaleSlope
		{
			get
			{
				return _realImageSop.RescaleSlope;
			}
			set
			{
				_realImageSop.RescaleSlope = value;
			}
		}

		public override string RescaleType
		{
			get
			{
				return _realImageSop.RescaleType;
			}
			set
			{
				_realImageSop.RescaleType = value;
			}
		}

		public override Window[] WindowCenterAndWidth
		{
			get
			{
				return _realImageSop.WindowCenterAndWidth;
			}
			set
			{
				_realImageSop.WindowCenterAndWidth = value;
			}
		}

		public override string[] WindowCenterAndWidthExplanation
		{
			get
			{
				return _realImageSop.WindowCenterAndWidthExplanation;
			}
			set
			{
				_realImageSop.WindowCenterAndWidthExplanation = value;
			}
		}

		public override void GetTag(DcmTagKey tag, out ushort val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out int val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out int val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out double val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out string val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out string val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTagArray(DcmTagKey tag, out string val, out bool tagExists)
		{
			_realImageSop.GetTagArray(tag, out val, out tagExists);
		}

		public override string SopInstanceUID
		{
			get
			{
				return _realImageSop.SopInstanceUID;
			}
			set
			{
				_realImageSop.SopInstanceUID = value;
			}
		}

		public override string TransferSyntaxUID
		{
			get
			{
				return _realImageSop.TransferSyntaxUID;
			}
			set
			{
				_realImageSop.TransferSyntaxUID = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
