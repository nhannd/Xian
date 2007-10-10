using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
        private string _cachedHtml;
        private ActionModelRenderer _renderer;


        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlComponentControl(IApplicationComponent component, ActiveTemplate template)
        {
            InitializeComponent();

            _component = component;
            _template = template;
            _renderer = new ActionModelRenderer();
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
            context["ActionModelRenderer"] = _renderer;
            _cachedHtml = _template.Evaluate(context);

            if (this.Visible)
            {
                _webBrowser.DocumentText = _cachedHtml;
            }
        }

        private void DisposedEventHandler(object sender, EventArgs e)
        {
            _component.AllPropertiesChanged -= AllPropertiesChangedEventHandler;
        }

        private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
        {
            ReloadPage();
        }

        private void _initialRefreshTimer_Tick(object sender, EventArgs e)
        {
            _initialRefreshTimer.Enabled = false;
            _webBrowser.DocumentText = _cachedHtml;
        }

        private void HtmlComponentControl_Load(object sender, EventArgs e)
        {
        }

        private void HtmlComponentControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                _initialRefreshTimer.Enabled = true;
            }
        }
    }
}
