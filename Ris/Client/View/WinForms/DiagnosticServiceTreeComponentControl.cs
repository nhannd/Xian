using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DiagnosticServiceTreeComponent"/>
    /// </summary>
    public partial class DiagnosticServiceTreeComponentControl : ApplicationComponentUserControl
    {
        private DiagnosticServiceTreeComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiagnosticServiceTreeComponentControl(DiagnosticServiceTreeComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _diagnosticServiceTree.Tree = _component.DiagnosticServiceTree;
            _diagnosticServiceTree.DataBindings.Add("Selection", _component, "SelectedDiagnosticServiceTreeItem", true, DataSourceUpdateMode.OnPropertyChanged);

            _procedures.Table = _component.DiagnosticServiceBreakdown;
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
