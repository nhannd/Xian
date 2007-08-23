using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.Samples.Cad
{
	public class CadOverlayGraphic : ColorImageGraphic
	{
		private int _threshold;
		private int _opacity;
		private GrayscaleImageGraphic _image;

		public CadOverlayGraphic(GrayscaleImageGraphic image) 
			: base(image.Rows, image.Columns)
		{
			_image = image;		
		}

		public int Threshold
		{
			get { return _threshold; }
			set { _threshold = value; }
		}

		public int Opacity
		{
			get { return _opacity; }
			set { _opacity = value; }
		}

		private Color OverlayColor
		{
			get
			{
				int alpha = (int)(this.Opacity / 100.0f * 255);
				return Color.FromArgb(alpha, Color.Red);
			}
		}

		public void Analyze()
		{
			this.PixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (_image.PixelData.GetPixel(pixelIndex) > this.Threshold)
							this.PixelData.SetPixel(pixelIndex, this.OverlayColor);
						else
							this.PixelData.SetPixel(pixelIndex, Color.Empty);
					});
		}
	}
}