using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
	static class IconCreator
	{
		private static readonly int _iconWidth = 100;
		private static readonly int _iconHeight = 100;

		public static Bitmap CreatePresentationImageIcon(IPresentationImage image)
		{
			Bitmap bmp = image.DrawToBitmap(_iconWidth, _iconHeight);
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);

			Pen pen = new Pen(Color.DarkGray);

			g.DrawRectangle(pen, 0, 0, _iconWidth - 1, _iconHeight - 1);

			pen.Dispose();
			g.Dispose();

			return bmp;
		}

		public static Bitmap CreateDisplaySetIcon(IDisplaySet displaySet)
		{
			Bitmap bmp = new Bitmap(_iconWidth, _iconHeight);
			int numImages = 3;
			float aspectRatio = _iconHeight / (float)_iconWidth;
			int offsetX = 10;
			int offsetY = (int)(aspectRatio * offsetX);
			int subIconWidth = _iconWidth - ((numImages - 1) * offsetX);
			int subIconHeight = (int)(aspectRatio * subIconWidth);

			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);

			IPresentationImage image = displaySet.PresentationImages[displaySet.PresentationImages.Count / 2];

			Pen pen = new Pen(Color.DarkGray);

			g.Clear(Color.Black);

			for (int i = 0; i < numImages; i++)
			{
				int x = offsetX * ((numImages - 1) - i);
				int y = offsetY * i;

				g.DrawImage(image.DrawToBitmap(subIconWidth, subIconHeight), new Point(x, y));
				g.DrawRectangle(pen, x, y, subIconWidth - 1, subIconHeight - 1);
			}

			pen.Dispose();
			g.Dispose();

			return bmp;
		}
	}
}
