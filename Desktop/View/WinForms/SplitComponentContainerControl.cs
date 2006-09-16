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

		public SplitComponentContainerControl(SplitComponentContainer component)
		{
			InitializeComponent();
			_component = component;

			SplitPane pane1 = _component.Pane1;
			SplitPane pane2 = _component.Pane2;

			if (component.SplitOrientation == SplitOrientation.Horizontal)
				_splitContainer.Orientation = Orientation.Horizontal;
			else
				_splitContainer.Orientation = Orientation.Vertical;

			Control control1 = pane1.ComponentHost.ComponentView.GuiElement as Control;
			Control control2 = pane2.ComponentHost.ComponentView.GuiElement as Control;

			_splitContainer.Panel1.Controls.Add(control1);
			_splitContainer.Panel2.Controls.Add(control2);
			control1.Dock = DockStyle.Fill;
			control2.Dock = DockStyle.Fill;
		}
	}
}
