#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ProgressBarGraphic
	{
		[Cloneable(true)]
		private class ContinuousProgressBarGraphic : ProgressBarGraphic
		{
			[CloneIgnore]
			private readonly Image _tray;

			[CloneIgnore]
			private readonly Image _bar;

			[CloneIgnore]
			private readonly Image _border;

			public ContinuousProgressBarGraphic()
			{
				_tray = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicTray.png");
				_bar = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicContinuousBar.png");
				_border = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBorder.png");
			}

			public override ProgressBarGraphicStyle Style
			{
				get { return ProgressBarGraphicStyle.Continuous; }
			}

			public override Size Size
			{
				get { return _tray.Size; }
			}

			protected override void RenderProgressBar(float progress, System.Drawing.Graphics g)
			{
				g.DrawImageUnscaledAndClipped(_tray, new Rectangle(Point.Empty, _tray.Size));
				using (Bitmap bar = new Bitmap(_bar))
				{
					int cols = _bar.Size.Width;
					int rows = _bar.Size.Height;
					int size = rows*cols;
					int max = (int) (progress*cols);
					for (int i = 0; i < size; i++)
					{
						if (i%cols > max)
							bar.SetPixel(i%cols, i/cols, Color.Transparent);
					}
					DrawImageCentered(g, bar);
				}
				DrawImageCentered(g, _border);
			}
		}
	}
}