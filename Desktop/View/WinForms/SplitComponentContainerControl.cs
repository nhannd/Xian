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

		public SplitComponentContainerControl(SplitComponentContainer component)
		{
			InitializeComponent();
			_component = component;

			SplitPane pane1 = _component.Pane1;
			SplitPane pane2 = _component.Pane2;

            // are we vertical?
            _vertical = _component.SplitOrientation == SplitOrientation.Vertical;

            // initialize the split ratio
            _splitRatio = pane1.Weight / (pane1.Weight + pane2.Weight);

            // assemble the split container control
            _splitContainer.Orientation = _vertical ? Orientation.Vertical : Orientation.Horizontal;

			Control control1 = pane1.ComponentHost.ComponentView.GuiElement as Control;
            control1.Dock = DockStyle.Fill;
            _splitContainer.Panel1.Controls.Add(control1);

            Control control2 = pane2.ComponentHost.ComponentView.GuiElement as Control;
            control2.Dock = DockStyle.Fill;
			_splitContainer.Panel2.Controls.Add(control2);

            // initialize the splitter distance
            ResetSplitterDistance();
        }

        private void _splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // when the user moves the splitter, we need to keep track of the split ratio
            if (!_resetting)
            {
                float x1 = _vertical ? _splitContainer.Panel1.Width : _splitContainer.Panel1.Height;
                float x2 = _vertical ? _splitContainer.Panel2.Width : _splitContainer.Panel2.Height;

                _splitRatio = x1 / (x1 + x2);
            }
        }

        private void SplitComponentContainerControl_SizeChanged(object sender, EventArgs e)
        {
            // when the size of the overall control changes, we adjust the splitter distance
            // so as to maintain the current splitRatio
            ResetSplitterDistance();
        }

        private void ResetSplitterDistance()
        {
            _resetting = true;

            float baseDimension = _vertical ? this.Width : this.Height;
            _splitContainer.SplitterDistance = (int)(_splitRatio * baseDimension);

            _resetting = false;
        }
	}
}
