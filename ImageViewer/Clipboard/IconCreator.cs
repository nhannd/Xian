#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
