#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class ActivityMonitorComponentControl : ApplicationComponentUserControl
	{
		private readonly ActivityMonitorComponent _component;

		public ActivityMonitorComponentControl(ActivityMonitorComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();


			_aeTitle.DataBindings.Add("Text", _component, "AeTitle");
			_hostName.DataBindings.Add("Text", _component, "HostName");
			_port.DataBindings.Add("Text", _component, "Port");
			_fileStore.DataBindings.Add("Text", _component, "FileStore");
			_totalStudies.DataBindings.Add("Text", _component, "TotalStudies");
			_failures.DataBindings.Add("Text", _component, "Failures");

			_diskSpace.DataBindings.Add("Text", _component, "DiskspaceUsed");
			_diskSpaceBar.DataBindings.Add("Value", _component, "DiskspaceUsedPercent");



			_workItemsTableView.Table = _component.WorkItemTable;
		}

		private void _searchButton_Click(object sender, System.EventArgs e)
		{
			_component.Search();
		}
	}
}
