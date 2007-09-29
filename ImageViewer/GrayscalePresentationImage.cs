using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A basic grayscale Presentation Image.
	/// </summary>
	public class GrayscalePresentationImage 
		: BasicPresentationImage, 
		IIndexedPixelDataProvider,
		IModalityLutProvider,
		IVoiLutProvider, 
		IColorMapProvider
	{
		/// <summary>
		/// Simple constructor, which will automatically create grayscale pixel data with the specified
		/// number of rows and columns.
		/// </summary>
		/// <param name="rows">the number of rows</param>
		/// <param name="columns">the number of columns</param>
		public GrayscalePresentationImage(int rows, int columns)
			: base(new GrayscaleImageGraphic(rows, columns))
		{
		}

		/// <summary>
		/// Constructor.  Allows more flexible construction of the image, allowing for the pixel data
		/// to be retrieved from and external source via a <see cref="PixelDataGetter"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="inverted"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <param name="pixelAspectRatioX"></param>
		/// <param name="pixelAspectRatioY"></param>
		/// <param name="pixelDataGetter"></param>
		public GrayscalePresentationImage(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY,
			PixelDataGetter pixelDataGetter)
			: base(new GrayscaleImageGraphic(
			       	rows,
			       	columns,
			       	bitsAllocated,
			       	bitsStored,
			       	highBit,
			       	isSigned,
			       	inverted,
			       	rescaleSlope,
			       	rescaleIntercept,
			       	pixelDataGetter),
			       pixelSpacingX,
			       pixelSpacingY,
			       pixelAspectRatioX,
			       pixelAspectRatioY)
		{

		}

		/// <summary>
		/// Gets the <see cref="GrayscaleImageGraphic"/> associated with this <see cref="GrayscalePresentationImage"/>.
		/// </summary>
		public new GrayscaleImageGraphic ImageGraphic
		{
			get { return (GrayscaleImageGraphic)base.ImageGraphic; }
		}

		/// <summary>
		/// Creates a clone of the <see cref="GrayscalePresentationImage"/>.
		/// </summary>
		/// <returns></returns>
		public override IPresentationImage Clone()
		{

			// TODO:
			return null;

			//IPresentationImage image = new BasicPresentationImage();
			//image.Uid = this.Uid;

			//return image;
		}

		#region IIndexedPixelDataProvider Members

		/// <summary>
		/// Gets this image's <see cref="IndexedPixelData"/>.
		/// </summary>
		public new IndexedPixelData PixelData
		{
			get { return ImageGraphic.PixelData; }
		}

		#endregion

		#region IModalityLutProvider Members

		/// <summary>
		/// Gets this image's <see cref="IModalityLut"/>.
		/// </summary>
		public IModalityLut ModalityLut
		{
			get
			{
				return this.ImageGraphic.ModalityLut;
			}
		}

		#endregion

		#region IVoiLutProvider Members

		/// <summary>
		/// Gets this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get 
			{
				return this.ImageGraphic.VoiLutManager;
			}
		}

		#endregion

		#region IColorMapProvider Members

		/// <summary>
		/// Gets this image's <see cref="IColorMapManager"/>.
		/// </summary>
		public IColorMapManager ColorMapManager
		{
			get
			{
				return this.ImageGraphic.ColorMapManager;
			}
		}

		#endregion
	}
}
