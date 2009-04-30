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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientReconciliationComponent"/>
    /// </summary>
    public partial class ReconciliationComponentControl : CustomUserControl
    {
        private ReconciliationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReconciliationComponentControl(ReconciliationComponent component)
        {
            InitializeComponent();

            _component = component;

            _targetTableView.Table = _component.TargetProfileTable;
            _reconciliationTableView.Table = _component.ReconciliationProfileTable;

            Control diffView = (Control)_component.DiffComponentView.GuiElement;
            diffView.Dock = DockStyle.Fill;
            _diffViewPanel.Controls.Add(diffView);

            _reconcileButton.DataBindings.Add("Enabled", _component, "ReconcileEnabled");
        }

        private void _reconcileButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Reconcile();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }


        private void _reconciliationTableView_SelectionChanged(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SetSelectedReconciliationProfile(_reconciliationTableView.Selection);
            }
        }

        private void _targetTableView_SelectionChanged(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SetSelectedTargetProfile(_targetTableView.Selection);
            }
        }

    }
}
