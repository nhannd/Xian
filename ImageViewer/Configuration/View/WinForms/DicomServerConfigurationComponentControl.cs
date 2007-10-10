using System;
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

            _aeTitle.DataBindings.Add("Value", _component, "AETitle", true, DataSourceUpdateMode.OnPropertyChanged);
            _port.DataBindings.Add("Value", _component, "Port", true, DataSourceUpdateMode.OnPropertyChanged);

            _aeTitle.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _port.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            _component.Refresh();
        }
    }
}
