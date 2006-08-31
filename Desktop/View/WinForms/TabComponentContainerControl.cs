using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TabComponentContainerControl : UserControl
	{
		TabComponentContainer _component;

		public TabComponentContainerControl(TabComponentContainer component)
		{
			InitializeComponent();
			_component = component;

			foreach (TabPage page in _component.Pages)
			{
				Control control = page.ComponentHost.ComponentView.GuiElement as Control;

				Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name, control);

				_tabControl.TabPages.Add(tabPageUI);
			}

		}

	}
}
