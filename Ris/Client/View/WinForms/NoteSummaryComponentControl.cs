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
    /// Provides a Windows Forms user-interface for <see cref="NoteSummaryComponent"/>
    /// </summary>
    public partial class NoteSummaryComponentControl : ApplicationComponentUserControl
    {
        private NoteSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteSummaryComponentControl(NoteSummaryComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;
            _noteList.Table = _component.Notes;
        }
    }
}
