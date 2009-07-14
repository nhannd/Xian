#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProcedureEditorComponent"/>
    /// </summary>
    public partial class ProcedureEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly ProcedureEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcedureEditorComponentControl(ProcedureEditorComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _procedureType.SuggestionProvider = _component.ProcedureTypeSuggestionProvider;
            _procedureType.Format += delegate(object sender, ListControlConvertEventArgs e)
                                         {
                                             e.Value = _component.FormatProcedureType(e.ListItem);
                                         };
            _procedureType.DataBindings.Add("Enabled", _component, "IsProcedureTypeEditable");
            _procedureType.DataBindings.Add("Value", _component, "SelectedProcedureType", true, DataSourceUpdateMode.OnPropertyChanged);

            _performingFacility.DataSource = _component.FacilityChoices;
            _performingFacility.DataBindings.Add("Value", _component, "SelectedFacility", true, DataSourceUpdateMode.OnPropertyChanged);
            _performingFacility.DataBindings.Add("Enabled", _component, "IsPerformingFacilityEditable");
            _performingFacility.Format += delegate(object sender, ListControlConvertEventArgs e)
                                         {
                                             e.Value = _component.FormatFacility(e.ListItem);
                                         };

            _laterality.DataSource = _component.LateralityChoices;
            _laterality.DataBindings.Add("Value", _component, "SelectedLaterality", true, DataSourceUpdateMode.OnPropertyChanged);

            _scheduledDate.DataBindings.Add("Value", _component, "ScheduledTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _scheduledDate.DataBindings.Add("Enabled", _component, "IsScheduledTimeEditable");
            _scheduledTime.DataBindings.Add("Value", _component, "ScheduledTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _scheduledTime.DataBindings.Add("Enabled", _component, "IsScheduledTimeEditable");

            _portable.DataBindings.Add("Checked", _component, "PortableModality", true, DataSourceUpdateMode.OnPropertyChanged);

            _checkedIn.DataBindings.Add("Checked", _component, "CheckedIn", true, DataSourceUpdateMode.OnPropertyChanged);
            _checkedIn.DataBindings.Add("Enabled", _component, "IsCheckedInEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
