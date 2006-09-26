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
    /// Provides a Windows Forms user-interface for <see cref="PatientPreviewComponent"/>
    /// </summary>
    public partial class PatientPreviewComponentControl : CustomUserControl
    {
        private PatientPreviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientPreviewComponentControl(PatientPreviewComponent component)
        {
            InitializeComponent();

            _component = component;

#if DEBUG
            _webBrowser.IsWebBrowserContextMenuEnabled = true;
#else
            _webBrowser.IsWebBrowserContextMenuEnabled = false;
#endif
            _webBrowser.DataBindings.Add("DocumentText", _component, "Html", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
