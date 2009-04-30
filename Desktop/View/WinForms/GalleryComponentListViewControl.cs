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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	internal class GalleryComponentListViewControl : ListView
	{
		private int _insertionBoxIndex = -1;
		private long _lastDragScrollEvent = 0;

		public int InsertionBoxIndex
		{
			get { return _insertionBoxIndex; }
			set
			{
				if (value < -1 || value >= base.Items.Count)
					value = -1;
				if (_insertionBoxIndex != value)
				{
					_insertionBoxIndex = value;
					base.Invalidate();
				}
			}
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			base.OnDragOver(drgevent);

			const int dragScrollDelay = 200; // in ms
			long now = DateTime.Now.Ticks/10000;
			if (base.Scrollable && now - _lastDragScrollEvent > dragScrollDelay)
			{
				Point clientPoint = base.PointToClient(new Point(drgevent.X, drgevent.Y));
				bool scrollUp = (clientPoint.Y < this.Height*3/20);
				bool scrollDown = (clientPoint.Y > this.Height*17/20);

				if (scrollUp || scrollDown)
				{
					int nearestIndex;
					ListViewItem lvi = base.GetItemAt(clientPoint.X, clientPoint.Y);
					if (lvi != null)
						nearestIndex = base.Items.IndexOf(lvi);
					else
						nearestIndex = base.InsertionMark.NearestIndex(clientPoint);

					if (nearestIndex >= 0)
					{
						_lastDragScrollEvent = now;
						if (scrollUp)
						{
							nearestIndex = Math.Max(nearestIndex - 1, 0);
						}
						else
						{
							nearestIndex = Math.Min(nearestIndex + 1, base.Items.Count - 1);
						}
						base.EnsureVisible(nearestIndex);
					}
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			const int WM_PAINT = 0x0f;
			if (m.Msg == WM_PAINT)
			{
				if (_insertionBoxIndex >= 0 && _insertionBoxIndex < base.Items.Count)
				{
					Rectangle rect = base.GetItemRect(_insertionBoxIndex, ItemBoundsPortion.Entire);
					using (Graphics g = base.CreateGraphics())
					{
						using (Pen p = new Pen(base.ForeColor, 2))
						{
							g.DrawRectangle(p, rect);
						}
					}
				}
			}
		}
	}
}