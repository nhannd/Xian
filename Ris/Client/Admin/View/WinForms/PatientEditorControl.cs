using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PatientEditorControl : UserControl
    {
        private PatientEditorComponent _component;

        public PatientEditorControl(PatientEditorComponent component)
        {
            InitializeComponent();
            _component = component;

            // create bindings
            _familyName.DataBindings.Add("Value", _component, "FamilyName", false, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", false, DataSourceUpdateMode.OnPropertyChanged);
            _middleName.DataBindings.Add("Value", _component, "MiddleName", false, DataSourceUpdateMode.OnPropertyChanged);

            _sex.DataSource = _component.SexChoices;
            _sex.DataBindings.Add("Value", _component, "Sex", false, DataSourceUpdateMode.OnPropertyChanged);

            _patientIdentifierList.DataSource = _component.PatientIdentifiers;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _identifierTableView_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetIdentifierSelection(_patientIdentifierList.CurrentSelection);            
        }

        private void _identiferAddButton_Click(object sender, EventArgs e)
        {
            _component.AddIdentifer();
        }

        private void _identifierUpdateButton_Click(object sender, EventArgs e)
        {
            _component.UpdateSelectedIdentifier(_patientIdentifierList.CurrentSelection);
        }

        private void _identifierDeleteButton_Click(object sender, EventArgs e)
        {
            _component.DeleteSelectedIdentifier(_patientIdentifierList.CurrentSelection);
        }

        private void _patientIdentifierList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedIdentifier(_patientIdentifierList.CurrentSelection);
        }

        private void PatientEditorControl_Load(object sender, EventArgs e)
        {
            _component.LoadIdentifierTable();
        }
    }
}
