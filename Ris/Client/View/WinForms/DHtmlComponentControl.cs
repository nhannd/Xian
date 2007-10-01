using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop;

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

            _webBrowser.DataBindings.Add("Url", _component, "HtmlPageUrl", true);
            _webBrowser.ObjectForScripting = _component.ScriptObject;
            _webBrowser.Navigating += NavigatingEventHandler;

            _component.Validation.Add(new ValidationRule("DUMMY_PROPERTY",
                delegate(IApplicationComponent c)
                {
                    object result = _webBrowser.Document.InvokeScript("hasValidationErrors");

                    // if result == null, the hasValidationErrors method is not implemented by the page
                    // in this case, assume there are no errors
                    bool hasErrors = (result == null) ? false : (bool)result;
                    return new ValidationResult(!hasErrors, "");
                }));

            _component.ValidationVisibleChanged += new EventHandler(_component_ValidationVisibleChanged);
            _component.DataSaving += new EventHandler(_component_DataSaving);
        }

        private void _component_DataSaving(object sender, EventArgs e)
        {
            _webBrowser.Document.InvokeScript("saveData", new object[] { _component.ValidationVisible });
        }

        private void _component_ValidationVisibleChanged(object sender, EventArgs e)
        {
            _webBrowser.Document.InvokeScript("showValidation", new object[] { _component.ValidationVisible });
        }

        private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
        {
            _webBrowser.Refresh();
        }

        private void NavigatingEventHandler(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.OriginalString.StartsWith("action:"))
            {
                e.Cancel = true;    // cancel the webbrowser navigation

                _component.InvokeAction(e.Url.LocalPath);
            }
        }
    }
}
