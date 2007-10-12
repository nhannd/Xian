#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
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

            _editorBrowser.Url = new Uri(_component.EditorUrl);
            _editorBrowser.ObjectForScripting = _component.ScriptObject;

            _previewBrowser.Url = new Uri(_component.PreviewUrl);
            _previewBrowser.ObjectForScripting = _component.ScriptObject;

            _patientName.DataBindings.Add("Text", _component, "PatientName", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _accessionNumber.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _visitNumber.DataBindings.Add("Value", _component, "VisitNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _dateOfBirth.DataBindings.Add("Value", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);
            _diagnosticService.DataBindings.Add("Value", _component, "DiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
            _requestedProcedure.DataBindings.Add("Value", _component, "RequestedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);
            _performedLocation.DataBindings.Add("Value", _component, "PerformedLocation", true, DataSourceUpdateMode.OnPropertyChanged);
            _performedDate.DataBindings.Add("Value", _component, "PerformedDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _dictateFor.DataBindings.Add("Value", _component, "SupervisorName", true, DataSourceUpdateMode.OnPropertyChanged);

            _residentPanel.DataBindings.Add("Visible", _component, "SupervisorSelectionVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            _dictateFor.DataBindings.Add("Visible", _component, "SupervisorSelectionVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            _chooseRadiologistButton.DataBindings.Add("Visible", _component, "SupervisorSelectionVisible", false, DataSourceUpdateMode.OnPropertyChanged);

            _verifyButton.DataBindings.Add("Enabled", _component, "VerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            _sendToVerifyButton.DataBindings.Add("Enabled", _component, "SendToVerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            _sendToTranscriptionButton.DataBindings.Add("Enabled", _component, "SendToTranscriptionEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

            if (_component.IsEditingAddendum == false)
            {
                _browserSplitContainer.Panel2Collapsed = true;
            }
        }

        private void _verifyButton_Click(object sender, EventArgs e)
        {
            _editorBrowser.Document.InvokeScript("saveData", null);
            _component.Verify();
        }

        private void _sendToVerifyButton_Click(object sender, EventArgs e)
        {
            _editorBrowser.Document.InvokeScript("saveData", null);
            _component.SendToVerify();
        }

        private void _sendToTranscriptionButton_Click(object sender, EventArgs e)
        {
            _editorBrowser.Document.InvokeScript("saveData", null);
            _component.SendToTranscription();
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            _editorBrowser.Document.InvokeScript("saveData", null);
            _component.Save();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _chooseRadiologistButton_Click(object sender, EventArgs e)
        {
            _component.ChooseRadiologist();
        }
    }
}
