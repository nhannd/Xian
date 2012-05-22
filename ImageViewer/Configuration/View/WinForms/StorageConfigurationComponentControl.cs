#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StorageConfigurationComponent"/>
    /// </summary>
    public partial class StorageConfigurationComponentControl : ApplicationComponentUserControl
    {
        private StorageConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public StorageConfigurationComponentControl(StorageConfigurationComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _fileStoreDirectory.DataBindings.Add("Text", _component, "FileStoreDirectory", true, DataSourceUpdateMode.OnPropertyChanged);

            _localServiceControlLink.DataBindings.Add("Text", _component, "LocalServiceControlLinkText", true, DataSourceUpdateMode.OnPropertyChanged);
            _localServiceControlLink.DataBindings.Add("Visible", _component, "IsLocalServiceControlLinkVisible", true, DataSourceUpdateMode.OnPropertyChanged);
            _localServiceControlLink.LinkClicked += (s,e) => _component.LocalServiceControlLinkClicked();

            _fileStoreWarningIcon.DataBindings.Add("Visible", _component, "HasFileStoreChanged", true, DataSourceUpdateMode.OnPropertyChanged);
            _fileStoreWarningMessage.DataBindings.Add("Visible", _component, "HasFileStoreChanged", true, DataSourceUpdateMode.OnPropertyChanged);
            _fileStoreWarningMessage.DataBindings.Add("Text", _component, "FileStoreChangedMessage", true, DataSourceUpdateMode.OnPropertyChanged);

            _totalDiskSpaceDisplay.DataBindings.Add("Text", _component, "TotalSpaceBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);

            var maxDiskUsageBinding = new Binding("Value", _component, "MaximumUsedSpacePercent", true, DataSourceUpdateMode.OnPropertyChanged);
			maxDiskUsageBinding.Parse += ParseDiskUsageBinding;
			maxDiskUsageBinding.Format += FormatDiskUsageBinding;

            _maxDiskSpaceDisplay.DataBindings.Add("Text", _component, "MaximumUsedSpaceDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
            _upDownMaxDiskSpace.DataBindings.Add("Value", _component, "MaximumUsedSpacePercent", true, DataSourceUpdateMode.OnPropertyChanged);
			_maxDiskSpace.DataBindings.Add(maxDiskUsageBinding);

            var usedSpaceMeterFillStateBinding = new Binding("FillState", _component, "IsMaximumUsedSpaceExceeded", true, DataSourceUpdateMode.OnPropertyChanged);
            usedSpaceMeterFillStateBinding.Parse += ParseMeterFillState;
            usedSpaceMeterFillStateBinding.Format += FormatMeterFillState;

            _usedSpaceMeter.DataBindings.Add(usedSpaceMeterFillStateBinding);
            _usedSpaceMeter.DataBindings.Add("Value", _component, "UsedSpacePercent", true, DataSourceUpdateMode.OnPropertyChanged);

            _usedDiskSpace.DataBindings.Add("Text", _component, "UsedSpacePercentDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
            _usedDiskSpaceDisplay.DataBindings.Add("Text", _component, "UsedSpaceBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);

            _diskSpaceWarningIcon.DataBindings.Add("Visible", _component, "IsMaximumUsedSpaceExceeded", true, DataSourceUpdateMode.OnPropertyChanged);
            _diskSpaceWarningMessage.DataBindings.Add("Visible", _component, "IsMaximumUsedSpaceExceeded", true, DataSourceUpdateMode.OnPropertyChanged);
            _diskSpaceWarningMessage.DataBindings.Add("Text", _component, "MaximumUsedSpaceExceededMessage", true, DataSourceUpdateMode.OnPropertyChanged);

			_deleteStudiesCheck.DataBindings.Add("Checked", _component, "DeleteStudies", true, DataSourceUpdateMode.OnPropertyChanged);

			_deleteTimeValue.DataBindings.Add("Value", _component, "DeleteTimeValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_deleteTimeValue.DataBindings.Add("Enabled", _component, "DeleteStudies");

			_deleteTimeUnits.Items.AddRange(_component.DeleteTimeUnits.Cast<object>().ToArray());
			_deleteTimeUnits.DataBindings.Add("SelectedItem", _component, "DeleteTimeUnit", true, DataSourceUpdateMode.OnPropertyChanged);
			_deleteTimeUnits.DataBindings.Add("Enabled", _component, "DeleteStudies");
			_deleteTimeUnits.Format += (sender, e) => { e.Value = _component.FormatTimeUnit(e.ListItem); };

            _changeFileStore.Click += (s, e) => _component.ChangeFileStore();
            _helpIcon.Click += (s,e) => _component.Help();
			
            
            _component.PropertyChanged += OnComponentPropertyChanged;

            //Set initial values.
            OnComponentPropertyChanged(this, new PropertyChangedEventArgs("FileStoreDirectory"));
            OnComponentPropertyChanged(this, new PropertyChangedEventArgs("FileStoreChangedDescription"));
            OnComponentPropertyChanged(this, new PropertyChangedEventArgs("MaximumUsedSpaceExceededDescription"));
            OnComponentPropertyChanged(this, new PropertyChangedEventArgs("HelpMessage"));
        }

        private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "FileStoreDirectory")
                _tooltip.SetToolTip(_fileStoreDirectory, _component.FileStoreDirectory);

            if (propertyChangedEventArgs.PropertyName == "FileStoreChangedDescription")
            {
                _tooltip.SetToolTip(_fileStoreWarningIcon, _component.FileStoreChangedDescription);
                _tooltip.SetToolTip(_fileStoreWarningMessage, _component.FileStoreChangedDescription);
            }

            if (propertyChangedEventArgs.PropertyName == "MaximumUsedSpaceExceededDescription")
            {
                _tooltip.SetToolTip(_diskSpaceWarningMessage, _component.MaximumUsedSpaceExceededDescription);
                _tooltip.SetToolTip(_diskSpaceWarningIcon, _component.MaximumUsedSpaceExceededDescription);
            }
            if (propertyChangedEventArgs.PropertyName == "HelpMessage")
            {
                _tooltip.SetToolTip(_helpIcon, _component.HelpMessage);
            }
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

        private void FormatDiskUsageBinding(object sender, ConvertEventArgs e)
		{
            double value = (double)e.Value;
			e.Value = (int)(value * 1000F);
		}

		private void ParseDiskUsageBinding(object sender, ConvertEventArgs e)
		{
			int value = (int)e.Value;
			e.Value = value / 1000.0;
		}
    }
}
