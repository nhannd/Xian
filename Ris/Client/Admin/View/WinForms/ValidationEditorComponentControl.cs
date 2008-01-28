using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ValidationEditorComponent"/>
    /// </summary>
    public partial class ValidationEditorComponentControl : ApplicationComponentUserControl
    {
        private ValidationEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationEditorComponentControl(ValidationEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _propertiesTableView.Table = _component.Rules;
            _propertiesTableView.DataBindings.Add("Selection", _component, "SelectedRule", true, DataSourceUpdateMode.OnPropertyChanged);
            _validationXml.DataBindings.Add("Text", _component, "RuleXml", true, DataSourceUpdateMode.OnPropertyChanged);
            _testButton.DataBindings.Add("Enabled", _component, "CanTestRules");
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _testButton_Click(object sender, EventArgs e)
        {
            _component.TestRules();
        }
    }
}
