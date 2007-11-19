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

            _searchField.DataBindings.Add("Value", _component, "SearchString", true, DataSourceUpdateMode.OnPropertyChanged);
            _searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Search();
            }
        }

        private void _searchResults_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.OpenPatient();
        }

    }
}
