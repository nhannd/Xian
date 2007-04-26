using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class SplitComponentContainerControl : UserControl
	{
		private SplitComponentContainer _component;
        private float _splitRatio;
        private bool _vertical;
        private bool _resetting;

		private bool PaneFixed
		{
			get { return (_component.Pane1.Fixed || _component.Pane2.Fixed); }
		}

		public SplitComponentContainerControl(SplitComponentContainer component)
		{
			_component = component;
			InitializeComponent();
			
			SplitPane pane1 = _component.Pane1;
			SplitPane pane2 = _component.Pane2;

            // are we vertical?
            _vertical = _component.SplitOrientation == SplitOrientation.Vertical;

            // assemble the split container control
            _splitContainer.Orientation = _vertical ? Orientation.Vertical : Orientation.Horizontal;

			Control control1 = pane1.ComponentHost.ComponentView.GuiElement as Control;
            control1.Dock = DockStyle.Fill;
            _splitContainer.Panel1.Controls.Add(control1);

            Control control2 = pane2.ComponentHost.ComponentView.GuiElement as Control;
			control2.Dock = DockStyle.Fill;
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
			float baseDimension = _vertical ? this.Width : this.Height;
			int maxDimensionPixels = 20;

			if (_component.Pane1.Fixed)
			{
				_splitContainer.FixedPanel = FixedPanel.Panel1;

				foreach (Control control in _splitContainer.Panel1.Controls[0].Controls)
					maxDimensionPixels = Math.Max(maxDimensionPixels, (_vertical ? control.Bounds.Right : control.Bounds.Bottom));

				_splitRatio = (maxDimensionPixels + 20) / baseDimension;
			}
			else if (_component.Pane2.Fixed)
			{
				_splitContainer.FixedPanel = FixedPanel.Panel2;

				foreach (Control control in _splitContainer.Panel2.Controls[0].Controls)
					maxDimensionPixels = Math.Max(maxDimensionPixels, (_vertical ? control.Bounds.Right : control.Bounds.Bottom));

				_splitRatio = 1F - (maxDimensionPixels + 20) / baseDimension;
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
