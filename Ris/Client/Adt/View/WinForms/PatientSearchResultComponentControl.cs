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
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientSearchResultComponent"/>
    /// </summary>
    public partial class PatientSearchResultComponentControl : CustomUserControl
    {
        private PatientSearchResultComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientSearchResultComponentControl(PatientSearchResultComponent component)
        {
            InitializeComponent();

            _component = component;

            _patientProfileTable.DataSource = _component.SearchResults;
        }

        private void _patientProfileTable_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelection(_patientProfileTable.CurrentSelection);
        }
    }
}
