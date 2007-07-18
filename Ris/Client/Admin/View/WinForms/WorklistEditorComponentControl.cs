using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="WorklistEditorComponent"/>
    /// </summary>
    public partial class WorklistEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly WorklistEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistEditorComponentControl(WorklistEditorComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

            _type.DataSource = _component.TypeChoices;
            _type.DataBindings.Add("Value", _component, "Type", true, DataSourceUpdateMode.OnPropertyChanged);

            _availableRequestedProcedureTypes.Table = _component.AvailableRequestedProcedureTypeGroups;
            _availableRequestedProcedureTypes.DataBindings.Add("Selection", _component, "AvailableRequestedProcedureTypeGroupsSelection", true, DataSourceUpdateMode.OnPropertyChanged);
            _selectedRequestedProcedureTypes.Table = _component.SelectedRequestedProcedureTypeGroups;
            _selectedRequestedProcedureTypes.DataBindings.Add("Selection", _component, "SelectedRequestedProcedureTypeGroupsSelection", true, DataSourceUpdateMode.OnPropertyChanged);

            _addButton.DataBindings.Add("Enabled", _component, "AddSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _removeButton.DataBindings.Add("Enabled", _component, "RemoveSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _component.RequestedProcedureTypeGroupTablesChanged += OnRequestedProcedureTypeGroupTablesChanged;
            SortTables();
        }

        private void OnRequestedProcedureTypeGroupTablesChanged(object sender, EventArgs e)
        {
            SortTables();
        }

        private void SortTables()
        {
            _availableRequestedProcedureTypes.Table.Sort();
            _selectedRequestedProcedureTypes.Table.Sort();
        }


        private void _acceptButton_Click(object sender, System.EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, System.EventArgs e)
        {
            _component.Cancel();
        }

        private void AddSelection(object sender, System.EventArgs e)
        {
            _component.AddSelection(_availableRequestedProcedureTypes.Selection);
        }

        private void RemoveSelection(object sender, System.EventArgs e)
        {
            _component.RemoveSelection(_selectedRequestedProcedureTypes.Selection);
        }
    }
}
