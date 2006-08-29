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
		public abstract string PatientsName { get; set; }
		public abstract string PatientId { get; set; }
		public abstract string PatientsBirthDate { get; set; }
		public abstract string PatientsSex { get; set; }

		// General Study Module
		public abstract string StudyInstanceUID { get; set; }
		public abstract string StudyDate { get; set; }
		public abstract string StudyTime { get; set; }
		public abstract string ReferringPhysiciansName { get; set; }
		public abstract string AccessionNumber { get; set; }
		public abstract string StudyDescription { get; set; }
		public abstract string NameOfPhysiciansReadingStudy { get; set; }

		// Patient Study Module
		public abstract string AdmittingDiagnosesDescription { get; set; }
		public abstract string PatientsAge { get; set; }
		public abstract string AdditionalPatientsHistory { get; set; }

		// General Series Module
		public abstract string Modality { get; set; }
		public abstract string SeriesInstanceUID { get; set; }
		public abstract string SeriesNumber { get; set; }
		public abstract string SeriesDescription { get; set; }
		public abstract string Laterality { get; set; }
		public abstract string SeriesDate { get; set; }
		public abstract string SeriesTime { get; set; }
		public abstract string PerformingPhysiciansName { get; set; }
		public abstract string OperatorsName { get; set; }
		public abstract string BodyPartExamined { get; set; }
		public abstract string PatientPosition { get; set; }

		// General Equipment Module
		public abstract string Manufacturer { get; set; }
		public abstract string InstitutionName { get; set; }
		public abstract string StationName { get; set; }
		public abstract string InstitutionalDepartmentName { get; set; }
		public abstract string ManufacturersModelName { get; set; }
		
		// General Image Module
		public abstract string InstanceNumber { get; set; }
		public abstract string PatientOrientationRows { get; set; }
		public abstract string PatientOrientationColumns { get; set; }
		public abstract string ImageType { get; set; }
		public abstract string AcquisitionNumber { get; set; }
		public abstract string AcquisitionDate { get; set; }
		public abstract string AcquisitionTime { get; set; }
		public abstract string AcquisitionDateTime { get; set; }
		public abstract string ImagesInAcquisition { get; set; }
		public abstract string ImageComments { get; set; }
		public abstract string LossyImageCompression { get; set; }
		public abstract string LossyImageCompressionRatio { get; set; }
		public abstract string PresentationLUTShape { get; set; }

		// Image Plane Module
		public abstract double PixelSpacingX { get; set; }
		public abstract double PixelSpacingY { get; set; }
		public abstract double ImageOrientationPatientRowX { get; set; }
		public abstract double ImageOrientationPatientRowY { get; set; }
		public abstract double ImageOrientationPatientRowZ { get; set; }
		public abstract double ImageOrientationPatientColumnX { get; set; }
		public abstract double ImageOrientationPatientColumnY { get; set; }
		public abstract double ImageOrientationPatientColumnZ { get; set; }
		public abstract double ImagePositionPatientX { get; set; }
		public abstract double ImagePositionPatientY { get; set; }
		public abstract double ImagePositionPatientZ { get; set; }
		public abstract double SliceThickness { get; set; }
		public abstract double SliceLocation { get; set; }
		public abstract double PixelAspectRatioX { get; set; }
		public abstract double PixelAspectRatioY { get; set; }

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
		public abstract double WindowCenter { get; set; }
		public abstract double WindowWidth { get; set; }
		public abstract double WindowCenterAndWidthExplanation { get; set; }

		public abstract void GetTag(DcmTagKey tag, out ushort val, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out double val, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists);
		public abstract void GetTag(DcmTagKey tag, out string val, out bool tagExists);

		public override string ToString()
		{
			return this.SopInstanceUID;
		}
	}
}
