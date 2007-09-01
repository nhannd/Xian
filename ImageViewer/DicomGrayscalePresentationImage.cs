using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Graphics;


namespace ClearCanvas.ImageViewer
{
	public class DicomGrayscalePresentationImage 
		: GrayscalePresentationImage, IImageSopProvider
	{
		private ImageSop _imageSop;

		public DicomGrayscalePresentationImage(ImageSop imageSop)
			: base(imageSop.Rows,
			       imageSop.Columns,
			       imageSop.BitsAllocated,
			       imageSop.BitsStored,
			       imageSop.HighBit,
			       imageSop.PixelRepresentation != 0 ? true : false,
			       imageSop.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ? true : false,
			       imageSop.RescaleSlope,
			       imageSop.RescaleIntercept,
			       imageSop.PixelSpacing.Column,
			       imageSop.PixelSpacing.Row,
			       imageSop.PixelAspectRatio.Column,
			       imageSop.PixelAspectRatio.Row,
			       imageSop.GetNormalizedPixelData)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_imageSop = imageSop;
			this.AnnotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);
		}

		#region IImageSopProvider members

		/// <summary>
		/// Gets this presentation image's associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ImageSop"/> to access DICOM tags.
		/// </remarks>
		public virtual ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		public override string ToString()
		{
			return _imageSop.InstanceNumber.ToString();
		}

		#endregion
	}
}
