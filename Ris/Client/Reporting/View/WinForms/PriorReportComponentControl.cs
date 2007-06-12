using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PriorReportComponent"/>
    /// </summary>
    public partial class PriorReportComponentControl : ApplicationComponentUserControl
    {
        private PriorReportComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PriorReportComponentControl(PriorReportComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }
    }
}
