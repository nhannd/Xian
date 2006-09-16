using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TabComponentContainerControl : UserControl
	{
		TabComponentContainer _component;

		public TabComponentContainerControl(TabComponentContainer component)
		{
			Platform.CheckForNullReference(component, "component");
			
			InitializeComponent();
			_component = component;

			_tabControl.ControlLeftOffset = 3;
			_tabControl.ControlTopOffset = 3;
			_tabControl.ControlRightOffset = 3;
			_tabControl.ControlBottomOffset = 3;

			foreach (TabPage page in _component.Pages)
			{
				Control control = page.ComponentHost.ComponentView.GuiElement as Control;

				Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name, control);
				tabPageUI.Tag = page;

				_tabControl.TabPages.Add(tabPageUI);
			}

			_tabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(OnControlSelectionChanged);
			_component.CurrentPageChanged += new EventHandler(OnComponentCurrentPageChanged);
		}

		void OnComponentCurrentPageChanged(object sender, EventArgs e)
		{
			TabPage currentTabPage = _tabControl.SelectedTab.Tag as TabPage;

			if (currentTabPage != _component.CurrentPage)
			{
				foreach (Crownwood.DotNetMagic.Controls.TabPage tabPageUI in _tabControl.TabPages)
				{
					TabPage tabPage = tabPageUI.Tag as TabPage;

					if (currentTabPage == tabPage)
						tabPageUI.Select();
				}
			}
		}

		void OnControlSelectionChanged(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
		{
			TabPage tabPage = newPage.Tag as TabPage;
			_component.CurrentPage = tabPage;
		}
	}
}
