using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Ris.Client.Admin;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PatientIdentifierEditorControl : UserControl
    {
        private PatientIdentifierEditorComponent _component;

        public PatientIdentifierEditorControl(PatientIdentifierEditorComponent component)
        {
            InitializeComponent();
            _component = component;

            _identifierType.DataSource = _component.TypeChoices;
            _identifierType.DataBindings.Add("Value", _component, "Type", true, DataSourceUpdateMode.OnPropertyChanged);

            _identifierID.DataBindings.Add("Value", _component, "Id", true, DataSourceUpdateMode.OnPropertyChanged);

            _identifierAssigningAuthority.DataSource = _component.AssigningAuthorityChoices;
            _identifierAssigningAuthority.DataBindings.Add("Value", _component, "AssigningAuthority", true, DataSourceUpdateMode.OnPropertyChanged);

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
