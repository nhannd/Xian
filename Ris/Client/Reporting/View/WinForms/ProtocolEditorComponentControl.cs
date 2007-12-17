#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
    /// Provides a Windows Forms user-interface for <see cref="ProtocolEditorComponent"/>
    /// </summary>
    public partial class ProtocolEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly ProtocolEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolEditorComponentControl(ProtocolEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            Control orderNotes = (Control)_component.OrderNotesComponentHost.ComponentView.GuiElement;
            orderNotes.Dock = DockStyle.Fill;
            _orderNotesPanel.Controls.Add(orderNotes);

            _protocolGroup.DataSource = _component.ProtocolGroupChoices;
            _protocolGroup.DataBindings.Add("Value", _component, "ProtocolGroup", true, DataSourceUpdateMode.OnPropertyChanged);
            _component.PropertyChanged += _component_PropertyChanged;

            _procedurePlanSummary.Table = _component.ProcedurePlanSummaryTable;
            _procedurePlanSummary.DataBindings.Add("Selection", _component, "SelectedRequestedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);
            _component.SelectionChanged += RefreshTables;

            _protocolNextItem.DataBindings.Add("Checked", _component, "ProtocolNextItem", true, DataSourceUpdateMode.OnPropertyChanged);
            _protocolNextItem.DataBindings.Add("Enabled", _component, "ProtocolNextItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            protocolCodesSelector.AvailableItemsTable = _component.AvailableProtocolCodesTable;
            protocolCodesSelector.SelectedItemsTable = _component.SelectedProtocolCodesTable;
            protocolCodesSelector.DataBindings.Add("Enabled", _component, "SaveEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnReject.DataBindings.Add("Enabled", _component, "RejectEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSuspend.DataBindings.Add("Enabled", _component, "SuspendEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSave.DataBindings.Add("Enabled", _component, "SaveEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSkip.DataBindings.Add("Enabled", _component, "SkipEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ProtocolGroupChoices")
            {
                _protocolGroup.DataSource = _component.ProtocolGroupChoices;
            }
        }

        private void RefreshTables(object sender, EventArgs e)
        {
            protocolCodesSelector.AvailableItemsTable = _component.AvailableProtocolCodesTable;
            protocolCodesSelector.SelectedItemsTable = _component.SelectedProtocolCodesTable;
        }

        private void btnAccept_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Accept();
            }
        }

        private void btnReject_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Reject();
            }
        }

        private void btnSuspend_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Suspend();
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Save();
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Close();
            }
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Skip();
            }
        }
    }
}
