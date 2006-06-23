using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Dashboard.View.WinForms
{
	[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Desktop.Dashboard.DashboardToolViewExtensionPoint))]
	public class DashboardView : WinFormsView, IToolView
	{
		DashboardTool _dashboardTool;
		DashboardForm _dashboardControl;

		public DashboardView()
		{
		}

		#region IToolView Members

		public void SetTool(ITool tool)
		{
			_dashboardTool = tool as DashboardTool;
		}

		#endregion

		public override object GuiElement
		{
			get { return this.Control; }
		}

		private DashboardForm Control
		{
			get
			{
				if (_dashboardControl == null)
					_dashboardControl = new DashboardForm();

				return _dashboardControl;
			}
		}
	}
}
