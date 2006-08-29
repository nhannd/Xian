using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PatientSearchControl : UserControl
    {
        private PatientSearchComponent _component;

        public PatientSearchControl(PatientSearchComponent component)
        {
            InitializeComponent();
            _component = component;

            _patientIdentifierType.DataSource = _component.PatientIdentifierTypeChoices;
            _patientIdentifierType.DataBindings.Add("Value", _component, "PatientIdentifierType", true, DataSourceUpdateMode.OnPropertyChanged);

            _patientIdentifier.DataBindings.Add("Value", _component, "PatientIdentifier", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            
            _sex.DataSource = _component.SexChoices;
            _sex.DataBindings.Add("Value", _component, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);

            _dateOfBirth.DataBindings.Add("Value", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);

            _searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled");
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            _component.Search();
        }

    }
}
