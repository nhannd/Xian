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

            _requestedProcedureTypesSelector.AvailableItemsTable = _component.AvailableRequestedProcedureTypes;
            _requestedProcedureTypesSelector.SelectedItemsTable = _component.SelectedRequestedProcedureTypes;

            _requestedProcedureTypesSelector.ItemAdded += OnItemsAddedOrRemoved;
            _requestedProcedureTypesSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _acceptButton.DataBindings.Add("Enabled", _component, "Modified", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void OnItemsAddedOrRemoved(object sender, EventArgs e)
        {
            _component.ItemsAddedOrRemoved();
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
