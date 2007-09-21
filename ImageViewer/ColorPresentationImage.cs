using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A color Presentation Image.
	/// </summary>
	public class ColorPresentationImage : BasicPresentationImage, IColorPixelDataProvider
	{
		/// <summary>
		/// Simple constructor, which will automatically create RGB pixel data with the specified
		/// number of rows and columns.
		/// </summary>
		/// <param name="rows">the number of rows</param>
		/// <param name="columns">the number of columns</param>
		public ColorPresentationImage(int rows, int columns) 
			: base(new ColorImageGraphic(rows, columns))
		{

		}

		/// <summary>
		/// Constructor.  Allows more flexible construction of the image, allowing for the pixel data
		/// to be retrieved from and external source via a <see cref="PixelDataGetter"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <param name="pixelAspectRatioX"></param>
		/// <param name="pixelAspectRatioY"></param>
		/// <param name="pixelDataGetter"></param>
		public ColorPresentationImage(
			int rows,
			int columns,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY,
			PixelDataGetter pixelDataGetter)
			: base(new ColorImageGraphic(rows, columns, pixelDataGetter),
			       pixelSpacingX,
			       pixelSpacingY,
			       pixelAspectRatioX,
			       pixelAspectRatioY)
		{

		}

		// TODO: Rename this method
		public override IPresentationImage Clone()
		{
			// TODO:
			return null;
		}

		/// <summary>
		/// Gets this image's <see cref="ColorImageGraphic"/>.
		/// </summary>
		public new ColorImageGraphic ImageGraphic
		{
			get { return (ColorImageGraphic)base.ImageGraphic;  }	
		}

		#region IColorPixelDataProvider Members

		/// <summary>
		/// Gets this image's <see cref="ColorPixelData"/>.
		/// </summary>
		public new ColorPixelData PixelData
		{
			get { return this.ImageGraphic.PixelData; }
		}

		#endregion
	}
}
