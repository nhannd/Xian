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

using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ProgressBarGraphic
	{
		[Cloneable(true)]
		private class BlocksProgressBarGraphic : ProgressBarGraphic
		{
			[CloneIgnore]
			private readonly Image _border;

			[CloneIgnore]
			private readonly Image _block;

			[CloneIgnore]
			private Bitmap _tray;

			[CloneIgnore]
			private readonly int _blockWidth;

			[CloneIgnore]
			private readonly int _blockCount;

			[CloneIgnore]
			private readonly int _blockOffsetY;

			public BlocksProgressBarGraphic()
			{
				_tray = new Bitmap(GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicTray.png"));
				_border = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBorder.png");
				_block = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBlocksUnit.png");

				_blockWidth = _block.Width;
				_blockCount = (int) Math.Ceiling(1f*_tray.Width/_blockWidth);
				_blockOffsetY = (_tray.Height - _block.Height)/2;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (_tray != null)
					{
						_tray.Dispose();
						_tray = null;
					}
				}

				base.Dispose(disposing);
			}

			public override ProgressBarGraphicStyle Style
			{
				get { return ProgressBarGraphicStyle.Blocks; }
			}

			public override Size Size
			{
				get { return _tray.Size; }
			}

			protected override void RenderProgressBar(float progress, System.Drawing.Graphics g)
			{
				int max = (int) (progress*_tray.Width);
				if (progress >= 1f)
					max = _blockCount*_blockWidth;

				using (Bitmap blocks = new Bitmap(_tray))
				{
					// draw the blocks to a temporary buffer
					using (System.Drawing.Graphics gBlocks = System.Drawing.Graphics.FromImage(blocks))
					{
						for (int x = 0; x < max; x += _blockWidth)
							gBlocks.DrawImageUnscaledAndClipped(_block, new Rectangle(new Point(x, _blockOffsetY), _block.Size));
					}

					// remask the blocks using the tray as a guideline
					int cols = _tray.Size.Width;
					int rows = _tray.Size.Height;
					int size = rows*cols;
					for (int i = 0; i < size; i++)
					{
						int x = i%cols;
						int y = i/cols;
						blocks.SetPixel(x, y, Color.FromArgb(_tray.GetPixel(x, y).A, blocks.GetPixel(x, y)));
					}

					// paint the temporary buffer onto the real buffer
					DrawImageCentered(g, blocks);
				}

				DrawImageCentered(g, _border);
			}
		}
	}
}