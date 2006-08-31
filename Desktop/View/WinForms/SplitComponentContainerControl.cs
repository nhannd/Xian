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

			SplitPane leftPane = _component.LeftPane;
			SplitPane rightPane = _component.RightPane;

			Control leftControl = leftPane.ComponentHost.ComponentView.GuiElement as Control;
			Control rightControl = rightPane.ComponentHost.ComponentView.GuiElement as Control;

			_splitContainer.Panel1.Controls.Add(leftControl);
			_splitContainer.Panel2.Controls.Add(rightControl);
			leftControl.Dock = DockStyle.Fill;
			rightControl.Dock = DockStyle.Fill;
		}
	}
}
