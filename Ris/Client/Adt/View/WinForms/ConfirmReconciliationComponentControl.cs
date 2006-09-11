using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ConfirmReconciliationComponent"/>
    /// </summary>
    public partial class ConfirmReconciliationComponentControl : UserControl
    {
        private ConfirmReconciliationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfirmReconciliationComponentControl(ConfirmReconciliationComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
            _sourceTable.DataSource = _component.SourcePatientData;
            _targetTable.DataSource = _component.TargetPatientData;

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
