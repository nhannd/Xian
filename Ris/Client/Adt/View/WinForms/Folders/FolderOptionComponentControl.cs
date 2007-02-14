using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.Folders.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderOptionComponent"/>
    /// </summary>
    public partial class FolderOptionComponentControl : ApplicationComponentUserControl
    {
        private FolderOptionComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderOptionComponentControl(FolderOptionComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            // TODO add .NET databindings to _component
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
