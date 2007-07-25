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
            _type.DataBindings.Add("Enabled", _component, "TypeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _availableRequestedProcedureTypes.Table = _component.AvailableRequestedProcedureTypeGroups;
            _availableRequestedProcedureTypes.DataBindings.Add("Selection", _component, "AvailableRequestedProcedureTypeGroupsSelection", true, DataSourceUpdateMode.OnPropertyChanged);
            _selectedRequestedProcedureTypes.Table = _component.SelectedRequestedProcedureTypeGroups;
            _selectedRequestedProcedureTypes.DataBindings.Add("Selection", _component, "SelectedRequestedProcedureTypeGroupsSelection", true, DataSourceUpdateMode.OnPropertyChanged);

            _addRequestedProceduerTypeButton.DataBindings.Add("Enabled", _component, "AddSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _removeRequestedProcedureTypeButton.DataBindings.Add("Enabled", _component, "RemoveSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _availableUsers.Table = _component.AvailableUsers;
            _availableUsers.DataBindings.Add("Selection", _component, "AvailableUsersSelection", true, DataSourceUpdateMode.OnPropertyChanged);
            _selectedUsers.Table = _component.SelectedUsers;
            _selectedUsers.DataBindings.Add("Selection", _component, "SelectedUsersSelection", true, DataSourceUpdateMode.OnPropertyChanged);

            _addUserButton.DataBindings.Add("Enabled", _component, "AddUserSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _removeUserButton.DataBindings.Add("Enabled", _component, "RemoveUserSelectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            
            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _component.RequestedProcedureTypeGroupTablesChanged += OnRequestedProcedureTypeGroupTablesChanged;
            _component.UserTablesChanged += OnUserTablesChanged;

            SortRequestedProcedureTypeGroupTables();
            SortUserTables();
        }

        private void OnRequestedProcedureTypeGroupTablesChanged(object sender, EventArgs e)
        {
            SortRequestedProcedureTypeGroupTables();
        }

        private void OnUserTablesChanged(object sender, EventArgs e)
        {
            SortUserTables();
        }

        private void SortRequestedProcedureTypeGroupTables()
        {
            _availableRequestedProcedureTypes.Table.Sort();
            _selectedRequestedProcedureTypes.Table.Sort();
        }

        private void SortUserTables()
        {
            _availableUsers.Table.Sort();
            _selectedUsers.Table.Sort();
        }


        private void _acceptButton_Click(object sender, System.EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, System.EventArgs e)
        {
            _component.Cancel();
        }

        private void AddRequestedProcedureTypeSelection(object sender, EventArgs e)
        {
            _component.AddRequestedProcedureTypeSelection(_availableRequestedProcedureTypes.Selection);
        }

        private void RemoveRequestedProcedureTypeSelection(object sender, EventArgs e)
        {
            _component.RemoveRequestedProcedureTypeSelection(_selectedRequestedProcedureTypes.Selection);
        }

        private void AddUserSelection(object sender, EventArgs e)
        {
            _component.AddUserSelection(_availableUsers.Selection);
        }

        private void RemoveUserSelection(object sender, EventArgs e)
        {
            _component.RemoveUserSelection(_selectedUsers.Selection);
        }
    }
}
