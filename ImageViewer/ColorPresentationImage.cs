using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer
{
	public class ColorPresentationImage 
		: BasicPresentationImage
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
	}
}
