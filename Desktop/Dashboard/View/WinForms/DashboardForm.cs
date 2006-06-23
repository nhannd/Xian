using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Dashboard
{
	public partial class DashboardForm : UserControl
	{
		public DashboardForm()
		{
			InitializeComponent();
			CreateDashboardContents();
		}

		private void CreateDashboardContents()
		{
            DashboardContentExtensionPoint xp = new DashboardContentExtensionPoint();
            object[] dashboardContents = xp.CreateExtensions();

			if (dashboardContents.Length == 0)
				return;

            foreach (IDashboardContent dashboardView in dashboardContents)
			{
				dashboardView.MasterViewHost = _outlookSidebar.UserPanel;
				dashboardView.DetailViewHost = _splitContainer1.Panel2;
				ToolStripButton item = new ToolStripButton();
				item.Text = dashboardView.Name;
				item.Padding = new Padding(2);
				item.Margin = new Padding(0);
				item.Height = 40;
				item.CheckOnClick = true;
				item.Click += OnButtonClick;
				item.Tag = dashboardView;
				_outlookSidebar.StackStrip.Items.Add(item);
			}

			_outlookSidebar.StackStrip.Items[0].PerformClick();
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			ToolStripItem item = sender as ToolStripItem;
			Platform.CheckForInvalidCast(item, "sender", "ToolStripItem");

            IDashboardContent view = item.Tag as IDashboardContent;
			_outlookSidebar.MainHeaderText = view.Name;

			view.OnSelected();
		}
	}

	[ExtensionPoint()]
	public class DashboardContentExtensionPoint : ExtensionPoint<IDashboardContent>
	{
	}
}