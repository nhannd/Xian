#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using System.Windows.Forms;
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

			this.DataBindings.Add("IsConnected", _component, "IsConnected");

			_aeTitle.DataBindings.Add("Text", _component, "AeTitle");
			this.DataBindings.Add("HostName", _component, "HostName");
			
			_totalStudies.DataBindings.Add("Text", _component, "TotalStudies");
			_failures.DataBindings.Add("Text", _component, "Failures");

			_diskSpace.DataBindings.Add("Text", _component, "DiskspaceUsed");
			_diskSpaceMeter.DataBindings.Add("Value", _component, "DiskspaceUsedPercent");
            Binding diskSpaceMeterFillStateBinding = new Binding("FillState", _component, "IsMaximumDiskspaceUsageExceeded", true, DataSourceUpdateMode.OnPropertyChanged);
            diskSpaceMeterFillStateBinding.Parse += ParseMeterFillState;
            diskSpaceMeterFillStateBinding.Format += FormatMeterFillState;
		    _diskSpaceMeter.DataBindings.Add(diskSpaceMeterFillStateBinding);

            _diskSpaceWarningIcon.DataBindings.Add("Visible", _component, "IsMaximumDiskspaceUsageExceeded", true, DataSourceUpdateMode.OnPropertyChanged);
            _diskSpaceWarningLabel.DataBindings.Add("Visible", _component, "IsMaximumDiskspaceUsageExceeded", true, DataSourceUpdateMode.OnPropertyChanged);
            _diskSpaceWarningLabel.DataBindings.Add("Text", _component, "DiskSpaceWarningLabel", true, DataSourceUpdateMode.OnPropertyChanged);

			// need to work with these manually, because data-binding doesn't work well with toolstrip comboboxes
			_activityFilter.Items.AddRange(_component.ActivityTypeFilterChoices.Cast<object>().Select(i => new FilterItem(i, _component.FormatActivityTypeFilter)).ToArray());
			_activityFilter.SelectedIndex = 0;
			_activityFilter.SelectedIndexChanged += _activityFilter_SelectedIndexChanged;

			// need to work with these manually, because data-binding doesn't work well with toolstrip comboboxes
			_statusFilter.Items.AddRange(_component.StatusFilterChoices.Cast<object>().Select(i => new FilterItem(i, _component.FormatStatusFilter)).ToArray());
			_statusFilter.SelectedIndex = 0;
			_statusFilter.SelectedIndexChanged += _statusFilter_SelectedIndexChanged;

			// need to work with these manually, because data-binding doesn't work well with toolstrip comboboxes
			_textFilter.TextChanged += _textFilter_TextChanged;

			_component.PropertyChanged += _component_PropertyChanged;

			_workItemsTableView.Table = _component.WorkItemTable;
		    _component.SetSelection(_workItemsTableView.Selection);
		    _workItemsTableView.SelectionChanged += (s, e) => _component.SetSelection(_workItemsTableView.Selection);
			UpdateTooltips();
		}

        private void FormatMeterFillState(object sender, ConvertEventArgs e)
        {
            bool isMaximumUsedSpaceExceeded = (bool)e.Value;
            e.Value = isMaximumUsedSpaceExceeded ? MeterFillState.Error : MeterFillState.Normal;
        }

        private void ParseMeterFillState(object sender, ConvertEventArgs e)
        {
            var meterState = (MeterFillState)e.Value;
            e.Value = meterState == MeterFillState.Error;
        }

		private void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "AeTitle":
				case "HostName":
				case "Port":
				case "FileStore":
                case "IsMaximumDiskspaceUsageExceeded":
					UpdateTooltips();
					break;
			}
		}

		public bool IsConnected
		{
			get { return _statusLight.Status == IndicatorLightStatus.Green; }
			set
			{
				_statusLight.Status = value ? IndicatorLightStatus.Green : IndicatorLightStatus.Red;
			}
		}

		public string HostName
		{
			get { return ""; }
			set
			{
				_hostName.Text = FormatHostAndPort();
			}
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

		private void _reindexLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			_component.StartReindex();
		}

		private void _openFileStoreLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			_component.OpenFileStore();
		}

		private void UpdateTooltips()
		{
			_toolTip.SetToolTip(_aeTitle, string.Format(SR.ActivityMonitorAeTitleToolTip, _component.AeTitle));
			_toolTip.SetToolTip(_hostName, string.Format(SR.ActivityMonitorHostPortToolTip, FormatHostAndPort()));
			_toolTip.SetToolTip(_openFileStoreLink, string.Format(SR.ActivityMonitorFileStoreToolTip, _component.FileStore));
            _toolTip.SetToolTip(_diskSpaceWarningLabel, _component.DiskSpaceWarningMessage);
        }

		private string FormatHostAndPort()
		{
			return string.Format("{0}:{1}", _component.HostName, _component.Port);
		}

	}
}
