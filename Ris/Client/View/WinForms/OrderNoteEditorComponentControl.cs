using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="OrderNoteEditorComponent"/>
    /// </summary>
    public partial class OrderNoteEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly OrderNoteEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderNoteEditorComponentControl(OrderNoteEditorComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _comment.DataBindings.Add("Value", _component, "Comment", true, DataSourceUpdateMode.OnPropertyChanged);
            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _comment.ReadOnly = !_component.IsNewItem;
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
