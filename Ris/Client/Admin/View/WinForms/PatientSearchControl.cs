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
        private PatientSearchTool _tool;

        public PatientSearchControl(PatientSearchTool tool)
        {
            InitializeComponent();
            _tool = tool;

            _patientIdentifierType.DataSource = _tool.PatientIdentifierTypeChoices;
            _patientIdentifierType.DataBindings.Add("Value", _tool, "PatientIdentifierType", false, DataSourceUpdateMode.OnPropertyChanged);

            _patientIdentifier.DataBindings.Add("Value", _tool, "PatientIdentifier", false, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Value", _tool, "FamilyName", false, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _tool, "GivenName", false, DataSourceUpdateMode.OnPropertyChanged);
            
            _sex.DataSource = _tool.SexChoices;
            _sex.DataBindings.Add("Value", _tool, "Sex", false, DataSourceUpdateMode.OnPropertyChanged);

            _searchButton.DataBindings.Add("Enabled", _tool, "SearchEnabled");
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            _tool.Search();
        }

    }
}
