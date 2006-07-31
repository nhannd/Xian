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
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);

            _sex.DataSource = _component.SexChoices;
            _sex.DataBindings.Add("Value", _component, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);

            _dateOfBirth.DataBindings.Add("Value", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);
            _dateOfDeath.DataBindings.Add("Value", _component, "TimeOfDeath", true, DataSourceUpdateMode.OnPropertyChanged);

            _patientIdentifierList.DataSource = _component.PatientIdentifiers;
            _patientIdentifierList.ToolbarModel = _component.PatientIdentifierToolbarActions;
            _patientIdentifierList.MenuModel = _component.PatientIdentifierMenuActions;
        }

        private void PatientEditorControl_Load(object sender, EventArgs e)
        {
            _component.LoadIdentifierTable();
        }

        private void _patientIdentifierList_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedIdentifier(_patientIdentifierList.CurrentSelection);
        }

        private void _identiferAddButton_Click(object sender, EventArgs e)
        {
            _component.AddIdentifer();
        }

        private void _identifierUpdateButton_Click(object sender, EventArgs e)
        {
            _component.UpdateSelectedIdentifier();
        }

        private void _patientIdentifierList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedIdentifier();
        }

        private void _identifierDeleteButton_Click(object sender, EventArgs e)
        {
            _component.DeleteSelectedIdentifier();
        }
    }
}
