using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class ImageSop : Sop
	{
		// Patient Module
		public abstract PersonName PatientsName { get; set; }
		public abstract string PatientId { get; set; }
		public abstract string PatientsBirthDate { get; set; }
		public abstract string PatientsSex { get; set; }

		// General Study Module
		public abstract string StudyInstanceUID { get; set; }
		public abstract string StudyDate { get; set; }
		public abstract string StudyTime { get; set; }
		public abstract PersonName ReferringPhysiciansName { get; set; }
		public abstract string AccessionNumber { get; set; }
		public abstract string StudyDescription { get; set; }
		public abstract PersonName[] NameOfPhysiciansReadingStudy { get; set; }

		// Patient Study Module
		public abstract string[] AdmittingDiagnosesDescription { get; set; }
		public abstract string PatientsAge { get; set; }
		public abstract string AdditionalPatientsHistory { get; set; }

		// General Series Module
		public abstract string Modality { get; set; }
		public abstract string SeriesInstanceUID { get; set; }
		public abstract int SeriesNumber { get; set; }
		public abstract string SeriesDescription { get; set; }
		public abstract string Laterality { get; set; }
		public abstract string SeriesDate { get; set; }
		public abstract string SeriesTime { get; set; }
		public abstract PersonName[] PerformingPhysiciansName { get; set; }
		public abstract PersonName[] OperatorsName { get; set; }
		public abstract string BodyPartExamined { get; set; }
		public abstract string PatientPosition { get; set; }

		// General Equipment Module
		public abstract string Manufacturer { get; set; }
		public abstract string InstitutionName { get; set; }
		public abstract string StationName { get; set; }
		public abstract string InstitutionalDepartmentName { get; set; }
		public abstract string ManufacturersModelName { get; set; }
		
		// General Image Module
		public abstract int InstanceNumber { get; set; }
		public abstract PatientOrientation PatientOrientation { get; set; }
		public abstract string ImageType { get; set; }
		public abstract int AcquisitionNumber { get; set; }
		public abstract string AcquisitionDate { get; set; }
		public abstract string AcquisitionTime { get; set; }
		public abstract string AcquisitionDateTime { get; set; }
		public abstract int ImagesInAcquisition { get; set; }
		public abstract string ImageComments { get; set; }
		public abstract string LossyImageCompression { get; set; }
		public abstract double[] LossyImageCompressionRatio { get; set; }
		public abstract string PresentationLUTShape { get; set; }

		// Image Plane Module
		public abstract PixelSpacing PixelSpacing { get; set; }
		public abstract ImageOrientationPatient ImageOrientationPatient { get; set; }
		public abstract ImagePositionPatient ImagePositionPatient { get; set; }
		public abstract double SliceThickness { get; set; }
		public abstract double SliceLocation { get; set; }
		public abstract PixelAspectRatio PixelAspectRatio { get; set; }

		// Image Pixel Module
		public abstract int SamplesPerPixel { get; set; }
		public abstract string PhotometricInterpretation { get; set; }
		public abstract int Rows { get; set; }
		public abstract int Columns { get; set; }
		public abstract int BitsAllocated { get; set; }
		public abstract int BitsStored { get; set; }
		public abstract int HighBit { get; set; }
		public abstract int PixelRepresentation { get; set; }
		public abstract byte[] GetPixelData();
		public abstract int PlanarConfiguration { get; set; }

		// Modality LUT Module
		public abstract double RescaleIntercept { get; set; }
		public abstract double RescaleSlope { get; set; }
		public abstract string RescaleType { get; set; }

		// VOI LUT Module
		public abstract Window[] WindowCenterAndWidth { get; set; }
		public abstract string[] WindowCenterAndWidthExplanation { get; set; }

		public abstract void GetTag(DcmTagKey tag, out ushort val, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists);

		public abstract void GetTag(DcmTagKey tag, out int val, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out int val, uint position, out bool tagExists);

		public abstract void GetTag(DcmTagKey tag, out double val, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists);
		
		public abstract void GetTag(DcmTagKey tag, out string val, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out string val, uint position, out bool tagExists);

		public abstract void GetTagArray(DcmTagKey tag, out string val, out bool tagExists);

		public override string ToString()
		{
			return this.SopInstanceUID;
		}
	}
}
