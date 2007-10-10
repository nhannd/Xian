using System;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

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
				Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name);
				tabPageUI.Tag = page;

				_tabControl.TabPages.Add(tabPageUI);
			}

			_tabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(OnControlSelectionChanged);
			_component.CurrentPageChanged += new EventHandler(OnComponentCurrentPageChanged);

            ShowPage(_component.CurrentPage);
		}

		private void OnComponentCurrentPageChanged(object sender, EventArgs e)
		{
            ShowPage(_component.CurrentPage);
		}

        private void OnControlSelectionChanged(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
		{
			TabPage tabPage = newPage.Tag as TabPage;
			_component.CurrentPage = tabPage;
		}

        private void ShowPage(TabPage page)
        {
            // find the tab corresponding to the current page
            Crownwood.DotNetMagic.Controls.TabPage tab = CollectionUtils.SelectFirst<Crownwood.DotNetMagic.Controls.TabPage>(_tabControl.TabPages,
                delegate(Crownwood.DotNetMagic.Controls.TabPage tp) { return tp.Tag == page; });

            // if the tab's control was not yet created, create it now
            if (tab.Control == null)
            {
                tab.Control = (Control)_component.GetPageView(page).GuiElement;
            }

            // ensure the correct tab is selected (in case the current page was changed programatically)
            tab.Select();
        }

    }
}
