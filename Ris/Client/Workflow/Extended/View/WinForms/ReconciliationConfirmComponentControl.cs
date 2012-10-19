#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ConfirmReconciliationComponent"/>
    /// </summary>
    public partial class ReconciliationConfirmComponentControl : CustomUserControl
    {
        private ReconciliationConfirmComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReconciliationConfirmComponentControl(ReconciliationConfirmComponent component)
        {
            InitializeComponent();

            _component = component;

            _sourceTable.Table = _component.SourcePatientData;
            _targetTable.Table = _component.TargetPatientData;
        }

        private void _continueButton_Click(object sender, EventArgs e)
        {
            _component.Continue();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
