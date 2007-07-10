using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="RegistrationPreviewComponent"/>
    /// </summary>
    public partial class RegistrationPreviewComponentControl : ApplicationComponentUserControl
    {
        private RegistrationPreviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationPreviewComponentControl(RegistrationPreviewComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _component.RefreshPreview += delegate { _browser.Refresh(); };
            _browser.ObjectForScripting = _component.ScriptObject;
            _browser.Url = new Uri(_component.DetailsPageUrl);
            _browser.ScriptErrorsSuppressed = true;
        }

        public event WebBrowserNavigatingEventHandler Navigating
        {
            add { _browser.Navigating += value; }
            remove { _browser.Navigating -= value; }
        }
    }
}
