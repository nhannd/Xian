using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class SearchComponentControl : CustomUserControl
    {
        private readonly SearchComponent _component;

        public SearchComponentControl(SearchComponent component)
        {
            InitializeComponent();
            _component = component;

            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Value", _component, "Healthcard", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _accessionNumber.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _showActive.DataBindings.Add("Checked", _component, "ShowActive", true, DataSourceUpdateMode.OnPropertyChanged);

            _mrn.DataBindings.Add("Visible", _component, "UseMrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Visible", _component, "UseHealthcard", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Visible", _component, "UseFamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Visible", _component, "UseGivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _accessionNumber.DataBindings.Add("Visible", _component, "UseAccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _showActive.DataBindings.Add("Visible", _component, "UseAccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            
            _keepOpen.DataBindings.Add("Checked", _component, "KeepOpen", true, DataSourceUpdateMode.OnPropertyChanged);
            _searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled");
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Search();
            }
        }

        private void SearchComponentControl_Load(object sender, EventArgs e)
        {
            _healthcard.Mask = _component.HealthcardMask;
        }

    }
}
