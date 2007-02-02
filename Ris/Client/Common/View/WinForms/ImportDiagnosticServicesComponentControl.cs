using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Common.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ImportDiagnosticServicesComponent"/>
    /// </summary>
    public partial class ImportDiagnosticServicesComponentControl : ApplicationComponentUserControl
    {
        private ImportDiagnosticServicesComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImportDiagnosticServicesComponentControl(ImportDiagnosticServicesComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _filename.DataBindings.Add("Value", _component, "FileName", true, DataSourceUpdateMode.OnPropertyChanged);
            _batchSize.DataBindings.Add("Value", _component, "BatchSize", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _browseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = ".";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _filename.Value = openFileDialog1.FileName;
            }
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            _component.OpenFile();
            _component.StartImport();
        }
    }
}
