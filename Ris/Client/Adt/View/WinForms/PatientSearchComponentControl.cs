using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    public partial class PatientSearchComponentControl : CustomUserControl
    {
        private PatientSearchComponent _component;

        public PatientSearchComponentControl(PatientSearchComponent component)
        {
            InitializeComponent();
            _component = component;

            _errorProvider.DataSource = _component;

            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Value", _component, "Healthcard", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            
            _sex.DataSource = _component.SexChoices;
            _sex.DataBindings.Add("Value", _component, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);

            _dateOfBirth.DataBindings.Add("Value", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);

            _searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled");
            _keepOpen.DataBindings.Add("Checked", _component, "KeepOpen", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            _component.Search();
        }

    }
}
