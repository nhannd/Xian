using System;
using ClearCanvas.Desktop.View.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientSearchComponent"/>
    /// </summary>
    public partial class PatientSearchComponentControl : ApplicationComponentUserControl
    {
        private readonly PatientSearchComponent _component;

        public PatientSearchComponentControl(PatientSearchComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _searchResults.ToolbarModel = _component.ItemsToolbarModel;
            _searchResults.MenuModel = _component.ItemsContextMenuModel;
            _searchResults.Table = _component.Profiles;
            _searchResults.DataBindings.Add("Selection", _component, "SelectedProfile", true, DataSourceUpdateMode.OnPropertyChanged);

            _sex.DataSource = _component.SexChoices;

            _mrn.DataBindings.Add("Value", _component, "MrnID", true, DataSourceUpdateMode.OnPropertyChanged);
            _site.DataBindings.Add("Value", _component, "MrnAssigningAuthority", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Value", _component, "Healthcard", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _sex.DataBindings.Add("Value", _component, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);
            _dateOfBirth.DataBindings.Add("Value", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);

            _searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            _component.Search();
        }

        private void _searchResults_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.OpenPatient();
        }
    }
}
