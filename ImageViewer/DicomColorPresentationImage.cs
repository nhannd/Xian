using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A Dicom colour presentation image.
	/// </summary>
	public class DicomColorPresentationImage
		: ColorPresentationImage, IImageSopProvider
	{
		private readonly ImageSop _imageSop;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomColorPresentationImage"/>.
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating an
		/// <see cref="ImageSop"/> with a <see cref="ColorPresentationImage"/>.
		/// </remarks>
		public DicomColorPresentationImage(ImageSop imageSop)
			: base(imageSop.Rows,
			       imageSop.Columns,
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

		public override IPresentationImage Clone()
		{
			return new DicomColorPresentationImage(_imageSop);
		}

		#region IImageSopProvider members

		/// <summary>
		/// Gets this presentation image's associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ImageSop"/> to access DICOM tags.
		/// </remarks>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		#endregion

		/// <summary>
		/// Returns the Instance Number as a string.
		/// </summary>
		/// <returns>The Instance Number as a string.</returns>
		public override string ToString()
		{
			return _imageSop.InstanceNumber.ToString();
		}
	}
}
