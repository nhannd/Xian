using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal class ImageSopProxy : ImageSop
	{
		private ImageSop _realImageSop;

		internal ImageSopProxy(ImageSop realImageSop)
		{
			Platform.CheckForNullReference(realImageSop, "realImageSop");
			_realImageSop = realImageSop;
		}

		public override DicomMessageBase NativeDicomObject
		{
			get 
			{ 
				return _realImageSop.NativeDicomObject; 
			}
		}

		public override string TransferSyntaxUID
		{
			get
			{
				return _realImageSop.TransferSyntaxUID;
			}
		}

		public override string SopInstanceUID
		{
			get
			{
				return _realImageSop.SopInstanceUID;
			}
		}

		public override PersonName PatientsName
		{
			get
			{
				return _realImageSop.PatientsName;
			}
		}

		public override string PatientId
		{
			get
			{
				return _realImageSop.PatientId;
			}
		}

		public override string PatientsBirthDate
		{
			get
			{
				return _realImageSop.PatientsBirthDate;
			}
		}

		public override string PatientsSex
		{
			get
			{
				return _realImageSop.PatientsSex;
			}
		}

		public override string StudyInstanceUID
		{
			get
			{
				return _realImageSop.StudyInstanceUID;
			}
		}

		public override string StudyDate
		{
			get
			{
				return _realImageSop.StudyDate;
			}
		}

		public override string StudyTime
		{
			get
			{
				return _realImageSop.StudyTime;
			}
		}

		public override PersonName ReferringPhysiciansName
		{
			get
			{
				return _realImageSop.ReferringPhysiciansName;
			}
		}

		public override string AccessionNumber
		{
			get
			{
				return _realImageSop.AccessionNumber;
			}
		}

		public override string StudyDescription
		{
			get
			{
				return _realImageSop.StudyDescription;
			}
		}

		public override PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				return _realImageSop.NameOfPhysiciansReadingStudy;
			}
		}

		public override string[] AdmittingDiagnosesDescription
		{
			get
			{
				return _realImageSop.AdmittingDiagnosesDescription;
			}
		}

		public override string PatientsAge
		{
			get
			{
				return _realImageSop.PatientsAge;
			}
		}

		public override string AdditionalPatientsHistory
		{
			get
			{
				return _realImageSop.AdditionalPatientsHistory;
			}
		}

		public override string Modality
		{
			get
			{
				return _realImageSop.Modality;
			}
		}

		public override string SeriesInstanceUID
		{
			get
			{
				return _realImageSop.SeriesInstanceUID;
			}
		}

		public override int SeriesNumber
		{
			get
			{
				return _realImageSop.SeriesNumber;
			}
		}

		public override string Laterality
		{
			get
			{
				return _realImageSop.Laterality;
			}
		}

		public override string SeriesDate
		{
			get
			{
				return _realImageSop.SeriesDate;
			}
		}

		public override string SeriesTime
		{
			get
			{
				return _realImageSop.SeriesTime;
			}
		}

		public override PersonName[] PerformingPhysiciansName
		{
			get
			{
				return _realImageSop.PerformingPhysiciansName;
			}
		}

		public override string ProtocolName
		{
			get
			{
				return _realImageSop.ProtocolName;
			}
		}

		public override string SeriesDescription
		{
			get
			{
				return _realImageSop.SeriesDescription;
			}
		}

		public override PersonName[] OperatorsName
		{
			get
			{
				return _realImageSop.OperatorsName;
			}
		}

		public override string BodyPartExamined
		{
			get
			{
				return _realImageSop.BodyPartExamined;
			}
		}

		public override string PatientPosition
		{
			get
			{
				return _realImageSop.PatientPosition;
			}
		}

		public override string Manufacturer
		{
			get
			{
				return _realImageSop.Manufacturer;
			}
		}

		public override string InstitutionName
		{
			get
			{
				return _realImageSop.InstitutionName;
			}
		}

		public override string StationName
		{
			get
			{
				return _realImageSop.StationName;
			}
		}

		public override string InstitutionalDepartmentName
		{
			get
			{
				return _realImageSop.InstitutionalDepartmentName;
			}
		}

		public override string ManufacturersModelName
		{
			get
			{
				return _realImageSop.ManufacturersModelName;
			}
		}

		public override int InstanceNumber
		{
			get
			{
				return _realImageSop.InstanceNumber;
			}
		}

		public override PatientOrientation PatientOrientation
		{
			get
			{
				return _realImageSop.PatientOrientation;
			}
		}

		public override string ImageType
		{
			get
			{
				return _realImageSop.ImageType;
			}
		}

		public override int AcquisitionNumber
		{
			get
			{
				return _realImageSop.AcquisitionNumber;
			}
		}

		public override string AcquisitionDate
		{
			get
			{
				return _realImageSop.AcquisitionDate;
			}
		}

		public override string AcquisitionTime
		{
			get
			{
				return _realImageSop.AcquisitionTime;
			}
		}

		public override string AcquisitionDateTime
		{
			get
			{
				return _realImageSop.AcquisitionDateTime;
			}
		}

		public override int ImagesInAcquisition
		{
			get
			{
				return _realImageSop.ImagesInAcquisition;
			}
		}

		public override string ImageComments
		{
			get
			{
				return _realImageSop.ImageComments;
			}
		}

		public override string LossyImageCompression
		{
			get
			{
				return _realImageSop.LossyImageCompression;
			}
		}

		public override double[] LossyImageCompressionRatio
		{
			get
			{
				return _realImageSop.LossyImageCompressionRatio;
			}
		}

		public override PixelSpacing PixelSpacing
		{
			get
			{
				return _realImageSop.PixelSpacing;
			}
		}

		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				return _realImageSop.ImageOrientationPatient;
			}
		}

		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				return _realImageSop.ImagePositionPatient;
			}
		}

		public override double SliceThickness
		{
			get
			{
				return _realImageSop.SliceThickness;
			}
		}

		public override double SliceLocation
		{
			get
			{
				return _realImageSop.SliceLocation;
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				return _realImageSop.PixelAspectRatio;
			}
		}

		public override int SamplesPerPixel
		{
			get
			{
				return _realImageSop.SamplesPerPixel;
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return _realImageSop.PhotometricInterpretation;
			}
		}

		public override int Rows
		{
			get
			{
				return _realImageSop.Rows;
			}
		}

		public override int Columns
		{
			get
			{
				return _realImageSop.Columns;
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return _realImageSop.BitsAllocated;
			}
		}

		public override int BitsStored
		{
			get
			{
				return _realImageSop.BitsStored;
			}
		}

		public override int HighBit
		{
			get
			{
				return _realImageSop.HighBit;
			}
		}

		public override int PixelRepresentation
		{
			get
			{
				return _realImageSop.PixelRepresentation;
			}
		}

		public override int PlanarConfiguration
		{
			get
			{
				return _realImageSop.PlanarConfiguration;
			}
		}

		public override double RescaleIntercept
		{
			get
			{
				return _realImageSop.RescaleIntercept;
			}
		}

		public override double RescaleSlope
		{
			get
			{
				return _realImageSop.RescaleSlope;
			}
		}

		public override string RescaleType
		{
			get
			{
				return _realImageSop.RescaleType;
			}
		}

		public override Window[] WindowCenterAndWidth
		{
			get
			{
				return _realImageSop.WindowCenterAndWidth;
			}
		}

		public override string[] WindowCenterAndWidthExplanation
		{
			get
			{
				return _realImageSop.WindowCenterAndWidthExplanation;
			}
		}

		public override byte[] GetNormalizedPixelData()
		{
			return _realImageSop.GetNormalizedPixelData();
		}

		public override void GetTag(uint tag, out ushort val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(uint tag, out ushort val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(uint tag, out int val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(uint tag, out int val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(uint tag, out double val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(uint tag, out double val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(uint tag, out string val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(uint tag, out string val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTagArray(uint tag, out string val, out bool tagExists)
		{
			_realImageSop.GetTagArray(tag, out val, out tagExists);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
