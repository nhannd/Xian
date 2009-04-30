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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class SplitComponentContainerControl : UserControl
	{
		private readonly SplitComponentContainer _component;
		private readonly bool _vertical;
		private bool _resetting;
		private float _splitRatio;
		
		private bool PaneFixed
		{
			get { return (_component.Pane1.Fixed || _component.Pane2.Fixed); }
		}

		public SplitComponentContainerControl(SplitComponentContainer component)
		{
			_component = component;

			_vertical = _component.SplitOrientation == SplitOrientation.Vertical;
			_resetting = false;

			SplitPane pane1 = _component.Pane1;
			SplitPane pane2 = _component.Pane2;

			Control control1 = (Control)pane1.ComponentHost.ComponentView.GuiElement;
            control1.Dock = DockStyle.Fill;

            Control control2 = (Control)pane2.ComponentHost.ComponentView.GuiElement;
			control2.Dock = DockStyle.Fill;

			control1.FontChanged += ContainedControlFontChanged;
			control2.FontChanged += ContainedControlFontChanged;

			InitializeComponent();

			// assemble the split container control
			_splitContainer.Orientation = _vertical ? Orientation.Vertical : Orientation.Horizontal;
			_splitContainer.Panel1.Controls.Add(control1);
			_splitContainer.Panel2.Controls.Add(control2);

			if (!PaneFixed)
			{
				// initialize the split ratio
				_splitRatio = pane1.Weight / (pane1.Weight + pane2.Weight);
			}
			else
			{
				FixPane();
			}

			// initialize the splitter distance
            ResetSplitterDistance();
		}

		private void ContainedControlFontChanged(object sender, EventArgs e)
		{
			FixPane();
			ResetSplitterDistance();
		}

        private void _splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
			if (PaneFixed || _resetting)
				return;

			// when the user moves the splitter, we need to keep track of the split ratio
			float x1 = _vertical ? _splitContainer.Panel1.Width : _splitContainer.Panel1.Height;
			float x2 = _vertical ? _splitContainer.Panel2.Width : _splitContainer.Panel2.Height;

			_splitRatio = x1 / (x1 + x2);
        }

        private void SplitComponentContainerControl_SizeChanged(object sender, EventArgs e)
        {
			if (PaneFixed)
				return;

			// when the size of the overall control changes, we adjust the splitter distance
            // so as to maintain the current splitRatio
			ResetSplitterDistance();
        }

		private void FixPane()
		{
			float baseDimension = _vertical ? _splitContainer.Width : _splitContainer.Height;
			int maxDimensionPixels = 20;

			if (_component.Pane1.Fixed)
			{
				_splitContainer.FixedPanel = FixedPanel.Panel1;

				int margin = 5 + (_vertical
				             	? _splitContainer.Panel1.Controls[0].Margin.Horizontal
				             	: _splitContainer.Panel1.Controls[0].Margin.Vertical);

				foreach (Control control in _splitContainer.Panel1.Controls[0].Controls)
					maxDimensionPixels = Math.Max(maxDimensionPixels, (_vertical ? control.Bounds.Right : control.Bounds.Bottom));

				_splitRatio = (maxDimensionPixels + margin) / baseDimension;
			}
			else if (_component.Pane2.Fixed)
			{
				_splitContainer.FixedPanel = FixedPanel.Panel2;

				int margin = 5 + (_vertical
								? _splitContainer.Panel2.Controls[0].Margin.Horizontal
								: _splitContainer.Panel2.Controls[0].Margin.Vertical);

				foreach (Control control in _splitContainer.Panel2.Controls[0].Controls)
					maxDimensionPixels = Math.Max(maxDimensionPixels, (_vertical ? control.Bounds.Right : control.Bounds.Bottom));

				_splitRatio = 1F - (maxDimensionPixels + margin) / baseDimension;
			}
		}
		
		private void ResetSplitterDistance()
        {
            int baseDimension = _vertical ? _splitContainer.Width : _splitContainer.Height;

			int min = _splitContainer.Panel1MinSize;
			int max = baseDimension - _splitContainer.Panel2MinSize;

			// rule is that SplitterDistance must be between Panel1MinSize and Width/Height - Panel2MinSize,
			// otherwise, setting the splitter distance will throw an exception.  So, we'll just let .NET take
			// care of it when the control is that small.
			if (max <= min)
				return;
			
			_resetting = true;
			
			_splitContainer.SplitterDistance = Bound(min, (int)(_splitRatio * baseDimension), max);

            _resetting = false;
        }

        private int Bound(int min, int val, int max)
        {
            return (val < min) ? min : (val > max) ? max : val;
        }
	}
}
