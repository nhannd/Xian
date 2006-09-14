using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientReconciliationComponent"/>
    /// </summary>
    public partial class PatientReconciliationComponentControl : CustomUserControl
    {
        private PatientReconciliationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientReconciliationComponentControl(PatientReconciliationComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
            _searchResultsTable.DataSource = _component.SearchResults;
            _alternateProfilesTable.DataSource = _component.AlternateProfiles;
            _reconciliationCandidateTable.DataSource = _component.ReconciliationCandidateProfiles;

            _mrnField.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardField.DataBindings.Add("Value", _component, "Healthcard", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyNameField.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenNameField.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _error.DataBindings.Add("Value", _component, "Error", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Search();
            }
        }

        private void _searchResultsTable_SelectionChanged(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SetSelectedSearchResults(_searchResultsTable.CurrentSelection);
            }
        }

        private void _reconcileButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Reconcile();
            }
        }
    }
}
