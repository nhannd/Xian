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

            _lbDriveName.DataSource = _component.AvailableDrives;
            _tbLowWatermark.DataBindings.Add("Value", _component, "LowWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
            _tbHighWatermark.DataBindings.Add("Value", _component, "HighWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
            _pbUsedSpace.DataBindings.Add("Value", _component, "SpaceUsed", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtLowWatermark.DataBindings.Add("Text", _component, "LowWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtHighWatermark.DataBindings.Add("Text", _component, "HighWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtUsedSpace.DataBindings.Add("Text", _component, "SpaceUsed", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtStatus.DataBindings.Add("Text", _component, "Status", true, DataSourceUpdateMode.OnPropertyChanged);

            _lbDriveName.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _tbLowWatermark.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _tbHighWatermark.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _pbUsedSpace.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtLowWatermark.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtHighWatermark.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtUsedSpace.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _txtStatus.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);

        }
    }
}
