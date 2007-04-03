using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="NoteCategoryEditorComponent"/>
    /// </summary>
    public partial class NoteCategoryEditorComponentControl : ApplicationComponentUserControl
    {
        private NoteCategoryEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategoryEditorComponentControl(NoteCategoryEditorComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _severity.DataSource = _component.SeverityChoices;
            _severity.DataBindings.Add("Value", _component, "Severity", true, DataSourceUpdateMode.OnPropertyChanged);
            _category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
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
