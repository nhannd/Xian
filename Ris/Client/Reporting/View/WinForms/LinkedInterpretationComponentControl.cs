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
    /// Provides a Windows Forms user-interface for <see cref="LinkedInterpretationComponent"/>
    /// </summary>
    public partial class LinkedInterpretationComponentControl : ApplicationComponentUserControl
    {
        private LinkedInterpretationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LinkedInterpretationComponentControl(LinkedInterpretationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _worklistItemTableView.Table = _component.CandidateTable;
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
