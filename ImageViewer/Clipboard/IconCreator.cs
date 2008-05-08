using System;
using System.Drawing;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard
{
	internal static class IconCreator
	{
		private static readonly int _iconWidth = 100;
		private static readonly int _iconHeight = 100;

		public static Bitmap CreatePresentationImageIcon(IPresentationImage image)
		{
			float aspectRatio = _iconHeight / (float)_iconWidth;

			int dimension = Math.Max(image.ClientRectangle.Width, image.ClientRectangle.Height);
			Bitmap img = image.DrawToBitmap(dimension, (int)(dimension * aspectRatio));
			Bitmap bmp = new Bitmap(img, _iconWidth, _iconHeight);
			
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
			img.Dispose();
			
			Pen pen = new Pen(Color.DarkGray);

			g.DrawRectangle(pen, 0, 0, _iconWidth - 1, _iconHeight - 1);

			pen.Dispose();
			g.Dispose();

			return bmp;
		}

		public static Bitmap CreateDisplaySetIcon(IDisplaySet displaySet, Rectangle displayedClientRectangle)
		{
			Bitmap bmp = new Bitmap(_iconWidth, _iconHeight);
			int numImages = 3;
			float aspectRatio = _iconHeight / (float)_iconWidth;
			int offsetX = 10;
			int offsetY = (int)(aspectRatio * offsetX);
			int subIconWidth = _iconWidth - ((numImages - 1) * offsetX);
			int subIconHeight = (int)(aspectRatio * subIconWidth);

			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);

			int dimension = Math.Max(displayedClientRectangle.Width, displayedClientRectangle.Height);

			IPresentationImage image = GetMiddlePresentationImage(displaySet);
			Bitmap img = image.DrawToBitmap(dimension, (int)(dimension * aspectRatio));
			Bitmap iconBmp = new Bitmap(img, subIconWidth, subIconHeight);
			img.Dispose();

			Pen pen = new Pen(Color.DarkGray);

			g.Clear(Color.Black);

			for (int i = 0; i < numImages; i++)
			{
				int x = offsetX * ((numImages - 1) - i);
				int y = offsetY * i;


				g.DrawImage(iconBmp, new Point(x, y));
				g.DrawRectangle(pen, x, y, subIconWidth - 1, subIconHeight - 1);
			}

			pen.Dispose();
			g.Dispose();

			return bmp;
		}

		private static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
		{
			if (displaySet.PresentationImages.Count == 0)
				throw new ArgumentException("The display set must contain at least one image.");

			if (displaySet.PresentationImages.Count <= 2)
				return displaySet.PresentationImages[0];

			return displaySet.PresentationImages[displaySet.PresentationImages.Count / 2];
		}
	}
}
