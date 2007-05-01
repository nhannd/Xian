using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using System.IO;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    public partial class DiskspaceManagerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DiskspaceManagerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagerConfigurationComponentControl(DiskspaceManagerConfigurationComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;
			
			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_txtDriveName.DataBindings.Add("Text", bindingSource, "DriveDisplay", true, DataSourceUpdateMode.OnPropertyChanged);

			Binding lowWatermarkBinding = new Binding("Value", bindingSource, "LowWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding highWatermarkBinding = new Binding("Value", bindingSource, "HighWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
			lowWatermarkBinding.Parse += new ConvertEventHandler(ParseWatermarkBinding);
			highWatermarkBinding.Parse += new ConvertEventHandler(ParseWatermarkBinding);
			lowWatermarkBinding.Format += new ConvertEventHandler(FormatWatermarkBinding);
			highWatermarkBinding.Format += new ConvertEventHandler(FormatWatermarkBinding);

			_txtLowWatermarkBytesDisplay.DataBindings.Add("Text", bindingSource, "LowWaterMarkBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownLowWatermark.DataBindings.Add("Value", bindingSource, "LowWaterMark", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbLowWatermark.DataBindings.Add(lowWatermarkBinding);

			_txtHighWatermarkBytesDisplay.DataBindings.Add("Text", bindingSource, "HighWaterMarkBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownHighWatermark.DataBindings.Add("Value", bindingSource, "HighWaterMark", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbHighWatermark.DataBindings.Add(highWatermarkBinding);

			_pbUsedSpace.DataBindings.Add("Value", bindingSource, "SpaceUsedPercent", true, DataSourceUpdateMode.OnPropertyChanged);
			_txtUsedSpace.DataBindings.Add("Text", bindingSource, "SpaceUsedPercentDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			_txtUsedSpaceBytesDisplay.DataBindings.Add("Text", bindingSource, "SpaceUsedBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			
			_txtFrequency.DataBindings.Add("Text", bindingSource, "CheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbFrequency.DataBindings.Add("Value", bindingSource, "CheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbFrequency.DataBindings.Add("Minimum", bindingSource, "MinimumCheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbFrequency.DataBindings.Add("Maximum", bindingSource, "MaximumCheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);

            _tbLowWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _tbHighWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _tbFrequency.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _pbUsedSpace.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownLowWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownHighWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_bnRefresh.Click += new System.EventHandler(OnRefreshClick);
        }

		private void FormatWatermarkBinding(object sender, ConvertEventArgs e)
		{
			float value = (float)e.Value;
			e.Value = (int)(value * 1000F);
		}

		private void ParseWatermarkBinding(object sender, ConvertEventArgs e)
		{
			int value = (int)e.Value;
			e.Value = value / 1000F;
		}

        private void OnRefreshClick(object sender, EventArgs e)
        {
            _component.Refresh();
        }
    }
}
