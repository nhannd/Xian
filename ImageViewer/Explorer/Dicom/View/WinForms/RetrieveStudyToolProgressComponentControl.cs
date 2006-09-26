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
    /// Provides a Windows Forms user-interface for <see cref="RetrieveStudyToolProgressComponent"/>
    /// </summary>
    public partial class RetrieveStudyToolProgressComponentControl : UserControl
    {
        private RetrieveStudyToolProgressComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RetrieveStudyToolProgressComponentControl(RetrieveStudyToolProgressComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }
    }
}
