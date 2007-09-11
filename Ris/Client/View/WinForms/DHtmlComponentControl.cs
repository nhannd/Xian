using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class DHtmlComponentControl : ApplicationComponentUserControl
    {
        private DHtmlComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DHtmlComponentControl(DHtmlComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

#if DEBUG
            _webBrowser.IsWebBrowserContextMenuEnabled = true;
#else
            _webBrowser.IsWebBrowserContextMenuEnabled = false;
#endif

            _component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
            this.Disposed += new EventHandler(DisposedEventHandler);

            _webBrowser.Url = new Uri(_component.DetailsPageUrl);
            _webBrowser.ObjectForScripting = _component.ScriptObject;
        }

        public event WebBrowserNavigatingEventHandler Navigating
        {
            add { _webBrowser.Navigating += value; }
            remove { _webBrowser.Navigating -= value; }
        }

        private void DisposedEventHandler(object sender, EventArgs e)
        {
            _component.AllPropertiesChanged -= AllPropertiesChangedEventHandler;
        }

        private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
        {
            _webBrowser.Refresh();
        }
    }
}
