using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="NoteEditorComponent"/>
    /// </summary>
    public partial class NoteEditorComponentControl : CustomUserControl
    {
        private NoteEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteEditorComponentControl(NoteEditorComponent component)
        {
            InitializeComponent();
            _component = component;

            _category.DataSource = _component.CategoryChoices;
            _category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "CategoryDescription", true, DataSourceUpdateMode.OnPropertyChanged);
            _comment.DataBindings.Add("Value", _component, "Comment", true, DataSourceUpdateMode.OnPropertyChanged);
            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
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
