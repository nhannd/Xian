using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM palette colour <see cref="PresentationImage"/>
	/// </summary>
	public class DicomPaletteColorPresentationImage
		: BasicPresentationImage, IImageSopProvider
	{
		private readonly ImageSop _imageSop;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomPaletteColorPresentationImage"/>.
		/// </summary>
		/// <param name="imageSop">The <see cref="ImageSop"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating an
		/// <see cref="ImageSop"/> with a <see cref="DicomPaletteColorPresentationImage"/>.
		/// </remarks>
		public DicomPaletteColorPresentationImage(ImageSop imageSop)
			: base(new PaletteColorImageGraphic(imageSop),
			       imageSop.PixelSpacing.Column,
			       imageSop.PixelSpacing.Row,
			       imageSop.PixelAspectRatio.Column,
			       imageSop.PixelAspectRatio.Row)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");
			_imageSop = imageSop;
			base.AnnotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);
		}

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

		/// <summary>
		/// Creates a fresh copy of the <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="DicomGrayscalePresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			return new DicomPaletteColorPresentationImage(_imageSop);
		}

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
