#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Drawing;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ProgressBarGraphic
	{
		[Cloneable(true)]
		private class MarqueeProgressBarGraphic : ProgressBarGraphic
		{
			[CloneIgnore]
			private SynchronizationContext _synchronizationContext;

			[CloneIgnore]
			private readonly Image _tray;

			[CloneIgnore]
			private readonly Image _bar;

			[CloneIgnore]
			private readonly Image _border;

			[CloneIgnore]
			private Bitmap _mask;

			[CloneIgnore]
			private volatile bool _isDisposed;

			[CloneIgnore]
			private volatile int _offset;

			[CloneIgnore]
			private readonly int _maskWidth;

			[CloneIgnore]
			private readonly int _barWidth;

			public MarqueeProgressBarGraphic()
			{
				_tray = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicTray.png");
				_bar = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicContinuousBar.png");
				_border = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBorder.png");
				_mask = new Bitmap(GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicMarqueeMask.png"));

				_maskWidth = _mask.Width;
				_barWidth = _bar.Width;
				_offset = -_maskWidth;
				_isDisposed = false;
			}

			protected override void Dispose(bool disposing)
			{
				_isDisposed = true;
				if (disposing)
				{
					if (_mask != null)
					{
						_mask.Dispose();
						_mask = null;
					}
				}

				base.Dispose(disposing);
			}

			public override Size Size
			{
				get { return _tray.Size; }
			}

			public override void OnDrawing()
			{
				if (_synchronizationContext == null)
					_synchronizationContext = SynchronizationContext.Current;

				bool isMarqueeLive = false;
				if (this.Progress > 0f && this.Progress < 1f)
					isMarqueeLive = (_synchronizationContext != null);

				base.OnDrawing();

				if (isMarqueeLive)
					_synchronizationContext.Post(Animate, null);
			}

			private void Animate(object state)
			{
				if (_isDisposed)
					return;

				_offset = ((_offset + _maskWidth + 3)%(_barWidth + _maskWidth)) - _maskWidth;
				this.Update();
			}

			protected override void RenderProgressBar(float progress, System.Drawing.Graphics g)
			{
				g.DrawImageUnscaledAndClipped(_tray, new Rectangle(Point.Empty, _tray.Size));

				if (progress <= 0f)
				{
					// paint nothing
				}
				else if (progress >= 1f)
				{
					// paint the full bar
					DrawImageCentered(g, _bar);
				}
				else
				{
					// paint a portion of the bar using the alpha channel of the marquee mask
					using (Bitmap bar = new Bitmap(_bar))
					{
						int cols = _bar.Size.Width;
						int rows = _bar.Size.Height;
						int size = rows*cols;

						for (int i = 0; i < size; i++)
						{
							int x = i%cols;
							int offsetX = x - _offset;
							int y = i/cols;
							if (offsetX >= 0 && offsetX < _mask.Width)
								bar.SetPixel(x, y, Color.FromArgb(_mask.GetPixel(offsetX, y).A, bar.GetPixel(x, y)));
							else
								bar.SetPixel(x, y, Color.Transparent);
						}
						DrawImageCentered(g, bar);
					}
				}
				DrawImageCentered(g, _border);
			}
		}
	}
}