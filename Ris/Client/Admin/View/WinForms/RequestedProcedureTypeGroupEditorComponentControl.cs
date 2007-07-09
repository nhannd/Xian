using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="RequestedProcedureTypeGroupEditorComponent"/>
    /// </summary>
    public partial class RequestedProcedureTypeGroupEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly RequestedProcedureTypeGroupEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RequestedProcedureTypeGroupEditorComponentControl(RequestedProcedureTypeGroupEditorComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

            _category.DataSource = _component.CategoryChoices;
            _category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);

            _availableRequestedProcedureTypes.Table = _component.AvailableRequestedProcedureTypes;
            _availableRequestedProcedureTypes.DataBindings.Add("Selection", _component, "AvailableRequestedProcedureTypesSelection", true, DataSourceUpdateMode.OnPropertyChanged);
            _selectedRequestedProcedureTypes.Table = _component.SelectedRequestedProcedureTypes;
            _selectedRequestedProcedureTypes.DataBindings.Add("Selection", _component, "SelectedRequestedProcedureTypesSelection", true, DataSourceUpdateMode.OnPropertyChanged);

            _addButton.DataBindings.Add("Enabled", _component, "AddSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _removeButton.DataBindings.Add("Enabled", _component, "RemoveSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _component.RequestedProcedureTypeTablesChanged += OnRequestedProcedureTypeTablesChanged;
            SortTables();
        }

        private void OnRequestedProcedureTypeTablesChanged(object sender, EventArgs e)
        {
            SortTables();
        }

        private void SortTables()
        {
            _availableRequestedProcedureTypes.Table.Sort();
            _selectedRequestedProcedureTypes.Table.Sort();
        }

        // Event handler for Add button and Available list double-click
        private void AddSelection(object sender, EventArgs e)
        {
            _component.AddSelection(_availableRequestedProcedureTypes.Selection);
        }

        // Event handler for Remove button and Selected list double-click
        private void RemoveSelection(object sender, EventArgs e)
        {
            _component.RemoveSelection(_selectedRequestedProcedureTypes.Selection);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
