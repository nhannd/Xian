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
    /// Provides a Windows Forms user-interface for <see cref="VisitLocationsSummaryComponent"/>
    /// </summary>
    public partial class VisitLocationsSummaryComponentControl : CustomUserControl
    {
        private VisitLocationsSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitLocationsSummaryComponentControl(VisitLocationsSummaryComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }
    }
}
