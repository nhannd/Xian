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

            _requestedProcedureTypeGroupsSelector.AvailableItemsTable = _component.AvailableRequestedProcedureTypeGroups;
            _requestedProcedureTypeGroupsSelector.SelectedItemsTable = _component.SelectedRequestedProcedureTypeGroups;
            _requestedProcedureTypeGroupsSelector.ItemAdded += OnItemsAddedOrRemoved;
            _requestedProcedureTypeGroupsSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _usersSelector.AvailableItemsTable = _component.AvailableUsers;
            _usersSelector.SelectedItemsTable = _component.SelectedUsers;
            _usersSelector.ItemAdded += OnItemsAddedOrRemoved;
            _usersSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _acceptButton.DataBindings.Add("Enabled", _component, "Modified", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void OnItemsAddedOrRemoved(object sender, EventArgs args)
        {
            _component.ItemsAddedOrRemoved();
        }

        private void _acceptButton_Click(object sender, System.EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, System.EventArgs e)
        {
            _component.Cancel();
        }
    }
}
