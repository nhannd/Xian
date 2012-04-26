#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
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

            var bindingSource = new BindingSource {DataSource = _component};

            _fileStoreDirectory.DataBindings.Add("Text", bindingSource, "FileStoreDirectory", true, DataSourceUpdateMode.OnPropertyChanged);

            Binding maxDiskUsageBinding = new Binding("Value", bindingSource, "MaximumUsedSpacePercent", true, DataSourceUpdateMode.OnPropertyChanged);
			maxDiskUsageBinding.Parse += ParseDiskUsageBinding;
			maxDiskUsageBinding.Format += FormatDiskUsageBinding;

            _maxDiskSpaceDisplay.DataBindings.Add("Text", bindingSource, "MaximumUsedSpaceDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
            _upDownMaxDiskSpace.DataBindings.Add("Value", bindingSource, "MaximumUsedSpacePercent", true, DataSourceUpdateMode.OnPropertyChanged);
			_maxDiskSpace.DataBindings.Add(maxDiskUsageBinding);

			_progressUsedDiskSpace.DataBindings.Add("Value", bindingSource, "UsedSpacePercent", true, DataSourceUpdateMode.OnPropertyChanged);
            _usedDiskSpace.DataBindings.Add("Text", bindingSource, "UsedSpacePercentDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
            _usedDiskSpaceDisplay.DataBindings.Add("Text", bindingSource, "UsedSpaceBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
        }

		private void FormatDiskUsageBinding(object sender, ConvertEventArgs e)
		{
			float value = (float)e.Value;
			e.Value = (int)(value * 1000F);
		}

		private void ParseDiskUsageBinding(object sender, ConvertEventArgs e)
		{
			int value = (int)e.Value;
			e.Value = value / 1000F;
		}

        private void _changeFileStore_Click(object sender, EventArgs e)
        {
            _component.ChangeFileStore();
        }
    }
}
