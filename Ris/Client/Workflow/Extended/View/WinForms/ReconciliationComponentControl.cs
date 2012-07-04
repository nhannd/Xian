#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
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
