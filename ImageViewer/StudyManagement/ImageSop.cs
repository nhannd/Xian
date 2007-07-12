using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Codecs;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM Image SOP Instance.
	/// </summary>
	public abstract class ImageSop : Sop
	{
		#region General Image Module

		/// <summary>
		/// Gets the instance number.
		/// </summary>
		public abstract int InstanceNumber { get; }

		/// <summary>
		/// Gets the patient orientation.
		/// </summary>
		public abstract PatientOrientation PatientOrientation { get; }

		/// <summary>
		/// Gets the image type.  The entire Image Type value should be returned as a Dicom string array.
		/// </summary>
		public abstract string ImageType { get; }

		/// <summary>
		/// Gets the acquisition number.
		/// </summary>
		public abstract int AcquisitionNumber { get; }

		/// <summary>
		/// Gets the acquisiton date.
		/// </summary>
		public abstract string AcquisitionDate { get; }

		/// <summary>
		/// Gets the acquisition time.
		/// </summary>
		public abstract string AcquisitionTime { get; }

		/// <summary>
		/// Gets the acquisition date/time.
		/// </summary>
		public abstract string AcquisitionDateTime { get; }

		/// <summary>
		/// Gets the number of images in the acquisition.
		/// </summary>
		public abstract int ImagesInAcquisition { get; }

		/// <summary>
		/// Gets the image comments.
		/// </summary>
		public abstract string ImageComments { get; }

		/// <summary>
		/// Gets the lossy image compression.
		/// </summary>
		public abstract string LossyImageCompression { get; }

		/// <summary>
		/// Gets the lossy image compression ratio.
		/// </summary>
		public abstract double[] LossyImageCompressionRatio { get; }

		/// <summary>
		/// Gets the presentation LUT shape.
		/// </summary>
		public abstract string PresentationLUTShape { get; }

		#endregion

		#region Image Plane Module

		/// <summary>
		/// Gets the pixel spacing.
		/// </summary>
		public abstract PixelSpacing PixelSpacing { get; }

		/// <summary>
		/// Gets the image orientation patient.
		/// </summary>
		public abstract ImageOrientationPatient ImageOrientationPatient { get; }

		/// <summary>
		/// Gets the image position patient.
		/// </summary>
		public abstract ImagePositionPatient ImagePositionPatient { get; }

		/// <summary>
		/// Gets the slice thickness.
		/// </summary>
		public abstract double SliceThickness { get; }

		/// <summary>
		/// Gets the slice location.
		/// </summary>
		public abstract double SliceLocation { get; }

		/// <summary>
		/// Gets the pixel aspect ratio.
		/// </summary>
		public abstract PixelAspectRatio PixelAspectRatio { get; }

		#endregion

		#region Image Pixel Module

		/// <summary>
		/// Gets the samples per pixel.
		/// </summary>
		public abstract int SamplesPerPixel { get; }

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		public abstract PhotometricInterpretation PhotometricInterpretation { get; }

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		public abstract int Rows { get; }

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		public abstract int Columns { get; }

		/// <summary>
		/// Gets the number of bits allocated.
		/// </summary>
		public abstract int BitsAllocated { get; }

		/// <summary>
		/// Gets the number of bits stored.
		/// </summary>
		public abstract int BitsStored { get; }

		/// <summary>
		/// Gets the high bit.
		/// </summary>
		public abstract int HighBit { get; }

		/// <summary>
		/// Gets the pixel representation.
		/// </summary>
		public abstract int PixelRepresentation { get; }

		/// <summary>
		/// Gets the planar configuration.
		/// </summary>
		public abstract int PlanarConfiguration { get; }

		#endregion

		#region Modality LUT Module

		/// <summary>
		/// Gets the rescale intercept.
		/// </summary>
		public abstract double RescaleIntercept { get; }

		/// <summary>
		/// Gets the rescale slope.
		/// </summary>
		public abstract double RescaleSlope { get; }

		/// <summary>
		/// Gets the rescale type.
		/// </summary>
		public abstract string RescaleType { get; }

		#endregion

		#region VOI LUT Module

		/// <summary>
		/// Gets the window width and center.
		/// </summary>
		public abstract Window[] WindowCenterAndWidth { get; }

		/// <summary>
		/// Gets the window width and center explanation.
		/// </summary>
		public abstract string[] WindowCenterAndWidthExplanation { get; }

		#endregion

		public abstract byte[] GetNormalizedPixelData();
	}
}
