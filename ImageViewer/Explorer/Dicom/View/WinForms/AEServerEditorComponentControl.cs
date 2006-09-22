using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AEServerEditorComponent"/>
    /// </summary>
    public partial class AEServerEditorComponentControl : UserControl
    {
        private AEServerEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public AEServerEditorComponentControl(AEServerEditorComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }
    }
}
