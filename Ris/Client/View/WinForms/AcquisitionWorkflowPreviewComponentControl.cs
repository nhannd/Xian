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
    /// Provides a Windows Forms user-interface for <see cref="AcquisitionWorkflowPreviewComponent"/>
    /// </summary>
    public partial class AcquisitionWorkflowPreviewComponentControl : ApplicationComponentUserControl
    {
        private AcquisitionWorkflowPreviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public AcquisitionWorkflowPreviewComponentControl(AcquisitionWorkflowPreviewComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }
    }
}
