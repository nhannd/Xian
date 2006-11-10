using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="VisitEditorComponent"/>
    /// </summary>
    public partial class VisitEditorComponentControl : CustomUserControl
    {
        private VisitEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitEditorComponentControl(VisitEditorComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }
    }
}
