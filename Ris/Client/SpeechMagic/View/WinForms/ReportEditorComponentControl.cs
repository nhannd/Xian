using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ReportEditorComponent"/>
    /// </summary>
    public partial class ReportEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly ReportEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportEditorComponentControl(ReportEditorComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            Control reportPreview = (Control)_component.ReportPreviewHost.ComponentView.GuiElement;
            reportPreview.Dock = DockStyle.Fill;
            _browserSplitContainer.Panel2.Controls.Add(reportPreview);

            _supervisor.LookupHandler = _component.SupervisorLookupHandler;
            _supervisor.DataBindings.Add("Value", _component, "Supervisor", true, DataSourceUpdateMode.OnPropertyChanged);

            _verifyButton.DataBindings.Add("Enabled", _component, "VerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            _sendToVerifyButton.DataBindings.Add("Enabled", _component, "SendToVerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            _sendToTranscriptionButton.DataBindings.Add("Enabled", _component, "SendToTranscriptionEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

            if (_component.CanVerifyReport)
            {
                _residentPanel.Visible = false;
                _supervisor.Visible = false;
            }
            else
            {
                _verifyButton.Visible = false;
            }

            if (_component.CanSendToTranscription == false)
                _sendToTranscriptionButton.Visible = false;

            if (_component.IsEditingAddendum == false)
            {
                _browserSplitContainer.Panel2Collapsed = true;
            }
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
