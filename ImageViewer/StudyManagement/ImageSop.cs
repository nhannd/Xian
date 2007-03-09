using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM image SOP.
	/// </summary>
	public abstract class ImageSop : Sop
	{
		/// <summary>
		/// Gets the underlying native DICOM object.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Sometimes, it is necessary to break the image SOP abstraction and expose
		/// the underlying implementation object, since providing a wrapper for the object
		/// in <see cref="ImageSop"/> would be prohibitive because of the large number of
		/// methods that would have to be wrapped.
		/// </para>
		/// <para>
		/// Because <see cref="NativeDicomObject"/> returns an <see cref="Object"/>, it
		/// needs to be cast to a known class.  Note that if the interface to that
		/// known class changes at some point in the future, client code may break.
		/// For this reason, <see cref="NativeDicomObject"/> should be used
		/// carefully and sparingly.
		/// </para>
		/// </remarks>
        public abstract object NativeDicomObject { get; }

		// Patient Module

		/// <summary>
		/// Gets or sets the patient's name.
		/// </summary>
		public abstract PersonName PatientsName { get; set; }

		/// <summary>
		/// Gets or sets the patient ID.
		/// </summary>
		public abstract string PatientId { get; set; }

		/// <summary>
		/// Gets or sets the patient's birthdate.
		/// </summary>
		public abstract string PatientsBirthDate { get; set; }

		/// <summary>
		/// Gets or sets the patient's sex.
		/// </summary>
		public abstract string PatientsSex { get; set; }

		// General Study Module

		/// <summary>
		/// Gets or sets the Study Instance UID.
		/// </summary>
		public abstract string StudyInstanceUID { get; set; }

		/// <summary>
		/// Gets or sets the study date.
		/// </summary>
		public abstract string StudyDate { get; set; }

		/// <summary>
		/// Gets or sets the study time.
		/// </summary>
		public abstract string StudyTime { get; set; }

		/// <summary>
		/// Gets or sets the referring physician's name.
		/// </summary>
		public abstract PersonName ReferringPhysiciansName { get; set; }

		/// <summary>
		/// Gets or sets the accession number.
		/// </summary>
		public abstract string AccessionNumber { get; set; }

		/// <summary>
		/// Gets or sets the study description.
		/// </summary>
		public abstract string StudyDescription { get; set; }

		/// <summary>
		/// Gets or sets the names of physicians reading the study.
		/// </summary>
		public abstract PersonName[] NameOfPhysiciansReadingStudy { get; set; }

		// Patient Study Module

		/// <summary>
		/// Gets or sets the admitting diagnoses descriptions.
		/// </summary>
		public abstract string[] AdmittingDiagnosesDescription { get; set; }

		/// <summary>
		/// Gets or sets the patient's age.
		/// </summary>
		public abstract string PatientsAge { get; set; }

		/// <summary>
		/// Gets or sets the additional patient's history.
		/// </summary>
		public abstract string AdditionalPatientsHistory { get; set; }

		// General Series Module

		/// <summary>
		/// Gets or sets the modality.
		/// </summary>
		public abstract string Modality { get; set; }

		/// <summary>
		/// Gets or sets the Series Instance UID.
		/// </summary>
		public abstract string SeriesInstanceUID { get; set; }

		/// <summary>
		/// Gets or sets the series number.
		/// </summary>
		public abstract int SeriesNumber { get; set; }

		/// <summary>
		/// Gets or sets the laterality.
		/// </summary>
		public abstract string Laterality { get; set; }

		/// <summary>
		/// Gets or sets the series date.
		/// </summary>
		public abstract string SeriesDate { get; set; }

		/// <summary>
		/// Gets or sets the series time.
		/// </summary>
		public abstract string SeriesTime { get; set; }

		/// <summary>
		/// Gets or sets the names of performing physicians.
		/// </summary>
		public abstract PersonName[] PerformingPhysiciansName { get; set; }

		/// <summary>
		/// Gets or sets the protocol name.
		/// </summary>
		public abstract string ProtocolName { get; set;}

		/// <summary>
		/// Gets or sets the series description.
		/// </summary>
		public abstract string SeriesDescription { get; set; }

		/// <summary>
		/// Gets or sets the names of operators.
		/// </summary>
		public abstract PersonName[] OperatorsName { get; set; }

		/// <summary>
		/// Gets or sets the body part examined.
		/// </summary>
		public abstract string BodyPartExamined { get; set; }

		/// <summary>
		/// Gets or sets the patient position.
		/// </summary>
		public abstract string PatientPosition { get; set; }

		// General Equipment Module

		/// <summary>
		/// Gets or sets the manufacturer.
		/// </summary>
		public abstract string Manufacturer { get; set; }

		/// <summary>
		/// Gets or sets the institution name.
		/// </summary>
		public abstract string InstitutionName { get; set; }

		/// <summary>
		/// Gets or sets the station name.
		/// </summary>
		public abstract string StationName { get; set; }

		/// <summary>
		/// Gets or sets the institutional department name.
		/// </summary>
		public abstract string InstitutionalDepartmentName { get; set; }

		/// <summary>
		/// Gets or sets the manufacturer's model name.
		/// </summary>
		public abstract string ManufacturersModelName { get; set; }
		
		// General Image Module

		/// <summary>
		/// Gets or sets the instance number.
		/// </summary>
		public abstract int InstanceNumber { get; set; }

		/// <summary>
		/// Gets or sets the patient orientation.
		/// </summary>
		public abstract PatientOrientation PatientOrientation { get; set; }

		/// <summary>
		/// Gets or sets the image type.
		/// </summary>
		public abstract string ImageType { get; set; }

		/// <summary>
		/// Gets or sets the acquisition number.
		/// </summary>
		public abstract int AcquisitionNumber { get; set; }

		/// <summary>
		/// Gets or sets the acquisiton date.
		/// </summary>
		public abstract string AcquisitionDate { get; set; }

		/// <summary>
		/// Gets or sets the acquisition time.
		/// </summary>
		public abstract string AcquisitionTime { get; set; }

		/// <summary>
		/// Gets or sets the acquisition date/time.
		/// </summary>
		public abstract string AcquisitionDateTime { get; set; }

		/// <summary>
		/// Gets or sets the number of images in the acquisition.
		/// </summary>
		public abstract int ImagesInAcquisition { get; set; }

		/// <summary>
		/// Gets or sets the image comments.
		/// </summary>
		public abstract string ImageComments { get; set; }

		/// <summary>
		/// Gets or sets the lossy image compression.
		/// </summary>
		public abstract string LossyImageCompression { get; set; }

		/// <summary>
		/// Gets or sets the lossy image compression ratio.
		/// </summary>
		public abstract double[] LossyImageCompressionRatio { get; set; }

		/// <summary>
		/// Gets or sets the presentation LUT shape.
		/// </summary>
		public abstract string PresentationLUTShape { get; set; }

		// Image Plane Module

		/// <summary>
		/// Gets or sets the pixel spacing.
		/// </summary>
		public abstract PixelSpacing PixelSpacing { get; set; }

		/// <summary>
		/// Gets or sets the image orientation patient.
		/// </summary>
		public abstract ImageOrientationPatient ImageOrientationPatient { get; set; }

		/// <summary>
		/// Gets or sets the image position patient.
		/// </summary>
		public abstract ImagePositionPatient ImagePositionPatient { get; set; }

		/// <summary>
		/// Gets or sets the slice thickness.
		/// </summary>
		public abstract double SliceThickness { get; set; }

		/// <summary>
		/// Gets or sets the slice location.
		/// </summary>
		public abstract double SliceLocation { get; set; }

		/// <summary>
		/// Gets or sets the pixel aspect ratio.
		/// </summary>
		public abstract PixelAspectRatio PixelAspectRatio { get; set; }

		// Image Pixel Module

		/// <summary>
		/// Gets or sets the samples per pixel.
		/// </summary>
		public abstract int SamplesPerPixel { get; set; }

		/// <summary>
		/// Gets or sets the photometric interpretation.
		/// </summary>
		public abstract PhotometricInterpretation PhotometricInterpretation { get; set; }

		/// <summary>
		/// Gets or sets the number of rows.
		/// </summary>
		public abstract int Rows { get; set; }

		/// <summary>
		/// Gets or sets the number of columns.
		/// </summary>
		public abstract int Columns { get; set; }

		/// <summary>
		/// Gets or sets the number of bits allocated.
		/// </summary>
		public abstract int BitsAllocated { get; set; }

		/// <summary>
		/// Gets or sets the number of bits stored.
		/// </summary>
		public abstract int BitsStored { get; set; }

		/// <summary>
		/// Gets or sets the high bit.
		/// </summary>
		public abstract int HighBit { get; set; }

		/// <summary>
		/// Gets or sets the pixel representation.
		/// </summary>
		public abstract int PixelRepresentation { get; set; }

		/// <summary>
		/// Gets the pixel data.
		/// </summary>
		public abstract byte[] PixelData { get; }

		/// <summary>
		/// Gets or sets the planar configuration.
		/// </summary>
		public abstract int PlanarConfiguration { get; set; }

		// Modality LUT Module

		/// <summary>
		/// Gets or sets the rescale intercept.
		/// </summary>
		public abstract double RescaleIntercept { get; set; }

		/// <summary>
		/// Gets or sets the rescale slope.
		/// </summary>
		public abstract double RescaleSlope { get; set; }

		/// <summary>
		/// Gets or sets the rescale type.
		/// </summary>
		public abstract string RescaleType { get; set; }

		// VOI LUT Module

		/// <summary>
		/// Gets or sets the window width and center.
		/// </summary>
		public abstract Window[] WindowCenterAndWidth { get; set; }

		/// <summary>
		/// Gets or sets the window width and center explanation.
		/// </summary>
		public abstract string[] WindowCenterAndWidthExplanation { get; set; }

		/// <summary>
		/// Gets a DICOM tag (16 bit, unsigned).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out ushort val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (16 bit, unsigned).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (integer).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out int val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (integer).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out int val, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (double).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out double val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (double).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (string).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out string val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (string).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out string val, uint position, out bool tagExists);

		public abstract void GetTagArray(DcmTagKey tag, out string val, out bool tagExists);

		/// <summary>
		/// Gets the pixel spacing appropriate to the modality.
		/// </summary>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <remarks>
		/// For projection based modalities (i.e., CR, DX and MG), Imager Pixel Spacing is
		/// returned as the pixel spacing.  For all other modalities, the standard
		/// Pixel Spacing is returned.
		/// </remarks>
		public void GetModalityPixelSpacing(out double pixelSpacingX, out double pixelSpacingY)
		{
			if (String.Compare(this.Modality, "CR", true) == 0 ||
				String.Compare(this.Modality, "DX", true) == 0 ||
				String.Compare(this.Modality, "MG", true) == 0)
			{
				bool tagExists;
				this.GetTag(Dcm.ImagerPixelSpacing, out pixelSpacingY, 0, out tagExists);

				if (!tagExists)
				{
					pixelSpacingX = double.NaN;
					pixelSpacingY = double.NaN;
					return;
				}

				this.GetTag(Dcm.ImagerPixelSpacing, out pixelSpacingX, 1, out tagExists);
			}
			else
			{
				pixelSpacingX = this.PixelSpacing.Row;
				pixelSpacingY = this.PixelSpacing.Column;
			}
		}

		public override string ToString()
		{
			return this.SopInstanceUID;
		}
	}
}
