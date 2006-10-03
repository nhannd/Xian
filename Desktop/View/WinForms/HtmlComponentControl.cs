using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientPreviewComponent"/>
    /// </summary>
    public partial class HtmlComponentControl : CustomUserControl
    {
        private IApplicationComponent _component;
        private ActiveTemplate _template;


        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlComponentControl(IApplicationComponent component, ActiveTemplate template)
        {
            InitializeComponent();

            _component = component;
            _template = template;

#if DEBUG
            _webBrowser.IsWebBrowserContextMenuEnabled = true;
#else
            _webBrowser.IsWebBrowserContextMenuEnabled = false;
#endif

            _component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
            this.Disposed += new EventHandler(DisposedEventHandler);
            ReloadPage();
        }

        public event WebBrowserNavigatingEventHandler Navigating
        {
            add { _webBrowser.Navigating += value; }
            remove { _webBrowser.Navigating -= value; }
        }

        internal void ReloadPage()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["Component"] = _component;

            _webBrowser.DocumentText = _template.Evaluate(context);
        }

        private void DisposedEventHandler(object sender, EventArgs e)
        {
            _component.AllPropertiesChanged -= AllPropertiesChangedEventHandler;
        }

        private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
        {
            ReloadPage();
        }
    }
}
