#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.StudyManagement;
using System;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class ActivityMonitorComponentControl : ApplicationComponentUserControl
	{
		class FilterItem
		{
			private readonly Func<object, string> _formatter;
			public FilterItem(object item, Func<object, string> formatter)
			{
				Item = item;
				_formatter = formatter;
			}

			public object Item { get; private set; }

			public override string ToString()
			{
				return _formatter(this.Item);
			}
		}


		private readonly ActivityMonitorComponent _component;

		public ActivityMonitorComponentControl(ActivityMonitorComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			ToolStripBuilder.BuildToolStrip(ToolStripBuilder.ToolStripKind.Toolbar, _workItemToolStrip.Items, _component.WorkItemActions.ChildNodes);

			_aeTitle.DataBindings.Add("Text", _component, "AeTitle");
			_hostName.DataBindings.Add("Text", _component, "HostName");
			_port.DataBindings.Add("Text", _component, "Port");
			_fileStore.DataBindings.Add("Text", _component, "FileStore");
			_totalStudies.DataBindings.Add("Text", _component, "TotalStudies");
			_failures.DataBindings.Add("Text", _component, "Failures");

			_diskSpace.DataBindings.Add("Text", _component, "DiskspaceUsed");
			_diskSpaceBar.DataBindings.Add("Value", _component, "DiskspaceUsedPercent");

			// need to work with these manually, because data-binding doesn't work well with toolstrip comboboxes
			_activityFilter.Items.AddRange(_component.ActivityTypeFilterChoices.Cast<object>().Select(i => new FilterItem(i, _component.FormatActivityTypeFilter)).ToArray());
			_activityFilter.SelectedIndex = 0;
			_activityFilter.SelectedIndexChanged += _activityFilter_SelectedIndexChanged;

			// need to work with these manually, because data-binding doesn't work well with toolstrip comboboxes
			_statusFilter.Items.AddRange(_component.StatusFilterChoices.Cast<object>().Select(i => new FilterItem(i, _component.FormatStatusFilter)).ToArray());
			_statusFilter.SelectedIndex = 0;
			_statusFilter.SelectedIndexChanged += _statusFilter_SelectedIndexChanged;

			_textFilter.TextChanged += _textFilter_TextChanged;

			_workItemsTableView.Table = _component.WorkItemTable;
		}

		private void _activityFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			_component.ActivityTypeFilter = ((FilterItem)_activityFilter.SelectedItem).Item;
		}

		private void _statusFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			_component.StatusFilter = ((FilterItem)_statusFilter.SelectedItem).Item;
		}

		private void _textFilter_TextChanged(object sender, EventArgs e)
		{
			_component.TextFilter = _textFilter.Text;
		}
	}
}
