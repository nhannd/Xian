using System;
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

            // TODO add .NET databindings to _component
        }

        private void _addButton_Click(object sender, EventArgs e)
        {

        }

        private void _removeButton_Click(object sender, EventArgs e)
        {

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
