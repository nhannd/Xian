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
using ClearCanvas.Common;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	internal sealed class IconHelper
	{
		public IconHelper() : this(new Size(100, 100), Color.DarkGray, 1) {}
		public IconHelper(int iconWidth, int iconHeight) : this(new Size(iconWidth, iconHeight)) {}
		public IconHelper(Size iconSize) : this(iconSize, Color.DarkGray, 1) {}
		public IconHelper(Color borderColor, int borderWidth) : this(new Size(100, 100), borderColor, borderWidth) {}
		public IconHelper(int iconWidth, int iconHeight, Color borderColor, int borderWidth) : this(new Size(iconWidth, iconHeight), borderColor, borderWidth) {}

		public IconHelper(Size iconSize, Color borderColor, int borderWidth)
		{
			_iconSize = iconSize;
			_borderColor = borderColor;
			_borderWidth = borderWidth;
		}

		#region Properties

		private Color _backgroundColor = Color.Black;

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set { _backgroundColor = value; }
		}

		private Color _borderColor;

		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		private int _borderWidth;

		public int BorderWidth
		{
			get { return _borderWidth; }
			set { _borderWidth = value; }
		}

		private Size _iconSize;

		public Size IconSize
		{
			get { return _iconSize; }
			set { _iconSize = value; }
		}

		private int _stackSize = 3;

		public int StackSize
		{
			get { return _stackSize; }
			set { _stackSize = value; }
		}

		private Size _stackOffset = new Size(10, 10);

		public Size StackOffset
		{
			get { return _stackOffset; }
			set { _stackOffset = value; }
		}

		public int StackHorizontalOffset
		{
			get { return _stackOffset.Width; }
			set { _stackOffset = new Size(value, _stackOffset.Height); }
		}

		public int StackVerticalOffset
		{
			get { return _stackOffset.Height; }
			set { _stackOffset = new Size(_stackOffset.Width, value); }
		}

		#endregion

		public Bitmap CreateImageIcon(Image image)
		{
			return GetImageIcon(delegate(int width, int height)
            	{
            		if (image == null)
            			return null;

					try {
						return new Bitmap(image, width, height);
					} catch (InvalidOperationException) {
						using (Image i2 = (Image)image.Clone()) {
							return new Bitmap(i2, width, height);
						}
					}
            	});
		}

		public Bitmap CreateImageIcon(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");
			return GetImageIcon(delegate(int width, int height) { return image.DrawToBitmap(width, height); });
		}

		public Bitmap CreateStackIcon(Image image)
		{
			return GetStackIcon(delegate(int width, int height)
            	{
            		if (image == null)
            			return null;

            		try
            		{
            			return new Bitmap(image, width, height);
            		}
            		catch (InvalidOperationException)
            		{
            			using (Image i2 = (Image) image.Clone())
            			{
            				return new Bitmap(i2, width, height);
            			}
            		}
            	});
		}

		public Bitmap CreateStackIcon(IDisplaySet displaySet)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			IPresentationImage image = GetMiddlePresentationImage(displaySet);
			return GetStackIcon(delegate(int width, int height) { return image.DrawToBitmap(width, height); });
		}

		public Bitmap CreateStackIcon(IImageSet imageSet)
		{
			Platform.CheckForNullReference(imageSet, "imageSet");
			IPresentationImage image = GetMiddlePresentationImage(imageSet);
			return GetStackIcon(delegate(int width, int height) { return image.DrawToBitmap(width, height); });
		}

		private delegate Bitmap BitmapCreatorDelegate(int width, int height);

		private Bitmap GetImageIcon(BitmapCreatorDelegate creator)
		{
			Bitmap icon = new Bitmap(_iconSize.Width, _iconSize.Height);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(icon))
			{
				using (Bitmap bmp = creator(_iconSize.Width, _iconSize.Height))
				{
					if (bmp != null)
						g.DrawImage(bmp, Point.Empty);
					else
						g.FillRectangle(new SolidBrush(_backgroundColor), new Rectangle(Point.Empty, icon.Size));
					DrawBorder(icon, _borderColor, _borderWidth);
				}
			}
			return icon;
		}

		private Bitmap GetStackIcon(BitmapCreatorDelegate creator)
		{
			Bitmap icon = new Bitmap(_iconSize.Width, _iconSize.Height);
			int subWidth = _iconSize.Width - (_stackSize - 1)*Math.Abs(_stackOffset.Width);
			int subHeight = _iconSize.Height - (_stackSize - 1)*Math.Abs(_stackOffset.Height);

			int offsetH = 0;
			if (_stackOffset.Width < 0)
			{
				offsetH = (_stackSize - 1)*Math.Abs(_stackOffset.Width);
			}

			int offsetV = 0;
			if (_stackOffset.Height < 0)
			{
				offsetV = (_stackSize - 1)*Math.Abs(_stackOffset.Height);
			}

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(icon))
			{
				using (Bitmap bmp = creator(subWidth, subHeight))
				{
					for (int n = _stackSize - 1; n >= 0; n--)
					{
						Point point = new Point(n*_stackOffset.Width + offsetH, n*_stackOffset.Height + offsetV);
						if (bmp != null)
							g.DrawImage(bmp, point);
						else
							g.FillRectangle(new SolidBrush(_backgroundColor), point.X, point.Y, subWidth, subHeight);
						DrawBorder(icon, new Rectangle(point, new Size(subWidth, subHeight)), _borderColor, _borderWidth);
					}
				}
			}
			return icon;
		}

		private static void DrawBorder(Image icon, Color penColor, int penWidth)
		{
			DrawBorder(icon, new Rectangle(Point.Empty, icon.Size), penColor, penWidth);
		}

		private static void DrawBorder(Image icon, Rectangle area, Color penColor, int penWidth)
		{
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(icon))
			{
				using (Pen pen = new Pen(penColor))
				{
					for (int n = 0; n < penWidth; n++)
					{
						g.DrawRectangle(pen, area.X + n, area.Y + n, area.Width - 2*n - 1, area.Height - 2*n - 1);
					}
				}
			}
		}

		private static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
		{
			if (displaySet.PresentationImages.Count == 0)
				throw new ArgumentException("The display set must contain at least one image.");
			return displaySet.PresentationImages[(displaySet.PresentationImages.Count - 1)/2];
		}

		private static IPresentationImage GetMiddlePresentationImage(IImageSet imageSet)
		{
			if (imageSet.DisplaySets.Count == 0)
				throw new ArgumentException("The image set must contain at least one display set.");
			return GetMiddlePresentationImage(imageSet.DisplaySets[0]);
		}
	}
}