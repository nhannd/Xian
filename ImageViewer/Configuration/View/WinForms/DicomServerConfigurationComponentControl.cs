using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    public partial class DicomServerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DicomServerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerConfigurationComponentControl(DicomServerConfigurationComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _hostName.DataBindings.Add("Value", _component, "HostName", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeTitle.DataBindings.Add("Value", _component, "AETitle", true, DataSourceUpdateMode.OnPropertyChanged);
            _port.DataBindings.Add("Value", _component, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
            _storageDir.DataBindings.Add("Value", _component, "StorageDir", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _buttonBrowse_Click(object sender, EventArgs e)
        {
            if (_storageDir.Value != "")
                folderBrowserDialog1.SelectedPath = _storageDir.Value;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _storageDir.Value = folderBrowserDialog1.SelectedPath;
            }

        }
    }
}
