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

namespace ClearCanvas.ImageViewer.View.WinForms
{
	partial class TrackSlider
	{
		// these aren't volatile because System.Windows.Forms.Timer uses the message pump to execute the tick on the UI thread
		private bool _thumbDrag = false;
		private bool _arrowHold = false;
		private int _arrowHoldCount = -1;
		private int _autoHideAlpha = 0;

		public void Flash()
		{
			if (_autoHideAlpha != 255)
			{
				_autoHideAlpha = 255;
				this.Invalidate();
			}
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			// don't do anything if this control isn't visible, or is not on an active form
			if (!this.Visible || this.Parent == null || Form.ActiveForm != this.FindForm())
				return;

			// perform sanity check now; do not allow actions if it fails check
			if (_minimumValue > _maximumValue)
				return;

			int alpha = _autoHideAlpha;

			if (_thumbDrag || !_autoHide)
			{
				// user is dragging the thumb slider, or we're not in auto hide mode
				alpha = 255;
			}
			else
			{
				// increase the opacity if the cursor is inside the control bounds
				if (this.RectangleToScreen(this.ClientRectangle).Contains(Cursor.Position))
					alpha = Math.Min(255, alpha + 5);
				else
					alpha = Math.Max(_minimumAlpha, alpha - 5);
			}

			if (_autoHideAlpha != alpha)
			{
				// we only need to invalidate and paint if the alpha changed
				_autoHideAlpha = alpha;
				this.Invalidate();
			}

			// if the user is holding down on the arrowheads, increment/decrement every now and then
			if (_arrowHold && _arrowHoldCount == 0)
				this.Value = this.ComputeTrackClickValue(this.PointToClient(Cursor.Position));
			_arrowHoldCount = (_arrowHoldCount + 1)%16;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			// perform sanity check now; do not allow actions if it fails check
			if (_minimumValue > _maximumValue)
				return;

			_thumbDrag = _trackBar.ThumbBounds.Contains(e.Location - this.PaddingOffset);
			if (!_thumbDrag)
			{
				this.Value = this.ComputeTrackClickValue(e.Location - this.PaddingOffset);
				_arrowHoldCount = 1;
				_arrowHold = true;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// perform sanity check now; do not allow actions if it fails check
			if (_minimumValue > _maximumValue)
				return;

			if (e.Button == MouseButtons.Left)
			{
				if (_thumbDrag)
					this.Value = this.ComputeThumbDragValue(e.Location - this.PaddingOffset);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_arrowHold = false;
			_thumbDrag = false;

			base.OnMouseUp(e);
		}

		/// <summary>
		/// Computes the value of the slider at the specified location.
		/// </summary>
		private int ComputeThumbDragValue(Point location)
		{
			float percentile;

			if (_orientation == Orientation.Vertical)
				percentile = 1f*(location.Y - _trackBar.TrackBounds.Top)/_trackBar.TrackBounds.Height;
			else
				percentile = 1f*(location.X - _trackBar.TrackBounds.Left)/_trackBar.TrackBounds.Width;

			percentile = Math.Min(1, Math.Max(0, percentile));
			return (int) Math.Round((_maximumValue - _minimumValue)*percentile + _minimumValue);
		}

		/// <summary>
		/// Computes the next value of the slider based on a clicked location.
		/// </summary>
		private int ComputeTrackClickValue(Point location)
		{
			if (_trackBar.TrackBounds.Contains(location))
				return this.ComputeThumbDragValue(location);
			else if (_trackBar.TrackStartBounds.Contains(location))
				return Math.Max(_minimumValue, Math.Min(_maximumValue, _value - _increment));
			else if (_trackBar.TrackEndBounds.Contains(location))
				return Math.Max(_minimumValue, Math.Min(_maximumValue, _value + _increment));
			else
				return _value;
		}
	}
}