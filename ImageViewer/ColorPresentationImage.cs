using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer
{
	public class ColorPresentationImage : BasicPresentationImage, IColorPixelDataProvider
	{
		public ColorPresentationImage(int rows, int columns) 
			: base(new ColorImageGraphic(rows, columns))
		{

		}

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
