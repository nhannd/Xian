using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ReportContentEditorComponent"/>
    /// </summary>
    public partial class ReportContentEditorComponentControl : ApplicationComponentUserControl
    {
        private ReportContentEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportContentEditorComponentControl(ReportContentEditorComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;
            
            _patientName.DataBindings.Add("Text", _component, "PatientName", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _accessionNumber.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _diagnosticService.DataBindings.Add("Value", _component, "DiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
            _requestedProcedure.DataBindings.Add("Value", _component, "RequestedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);
            _priority.DataBindings.Add("Value", _component, "Priority", true, DataSourceUpdateMode.OnPropertyChanged);

            _report.DataBindings.Add("Value", _component, "Report", true, DataSourceUpdateMode.OnPropertyChanged);
            _report.DataBindings.Add("ReadOnly", _component, "ReadOnly", true, DataSourceUpdateMode.OnPropertyChanged);
            _verifyButton.DataBindings.Add("Enabled", _component, "VerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            _sendToVerifyButton.DataBindings.Add("Enabled", _component, "SendToVerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            _sendToTranscriptionButton.DataBindings.Add("Enabled", _component, "SendToTranscriptionEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            _saveButton.DataBindings.Add("Enabled", _component, "SaveEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _verifyButton_Click(object sender, EventArgs e)
        {
            _component.Verify();
        }

        private void _sendToVerifyButton_Click(object sender, EventArgs e)
        {
            _component.SendToVerify();
        }

        private void _sendToTranscriptionButton_Click(object sender, EventArgs e)
        {
            _component.SendToTranscription();
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            _component.Save();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
