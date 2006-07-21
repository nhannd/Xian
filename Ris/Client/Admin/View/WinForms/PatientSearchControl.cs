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

            _mrn.DataBindings.Add("Value", _tool, "Mrn");
            _familyName.DataBindings.Add("Value", _tool, "FamilyName");
            _givenName.DataBindings.Add("Value", _tool, "GivenName");

            _searchButton.DataBindings.Add("Enabled", _tool, "SearchEnabled");
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            _tool.Search();
        }

    }
}
