#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class DHtmlComponentControl : ApplicationComponentUserControl
    {
        private readonly DHtmlComponent _component;

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
			
			//_webBrowser.DataBindings.Add("Url", _component, "HtmlPageUrl", true, DataSourceUpdateMode.OnPropertyChanged);
            _webBrowser.ObjectForScripting = _component.ScriptObject;
            _webBrowser.Navigating += NavigatingEventHandler;
            _webBrowser.ScrollBarsEnabled = _component.ScrollBarsEnabled;
			if (_component.HtmlPageUrl != null)
			{
				_webBrowser.Navigate(_component.HtmlPageUrl);
			}


            _component.Validation.Add(new ValidationRule("DUMMY_PROPERTY",
                delegate
                {
					var result = _webBrowser.Document != null ? _webBrowser.Document.InvokeScript("hasValidationErrors") : null;

                    // if result == null, the hasValidationErrors method is not implemented by the page
                    // in this case, assume there are no errors
                    var hasErrors = (result == null) ? false : (bool)result;
                    return new ValidationResult(!hasErrors, "");
                }));

            _component.ValidationVisibleChanged += _component_ValidationVisibleChanged;
            _component.DataSaving += _component_DataSaving;

        	_component.PrintDocumentRequested += _component_PrintDocument;
			_component.AsyncInvocationCompleted += _component_AsyncInvocationCompleted;
			_component.AsyncInvocationError += _component_AsyncInvocationError;
        }

        private void _component_PrintDocument(object sender, EventArgs e)
        {
            _webBrowser.Print();
        }

        private void _component_DataSaving(object sender, EventArgs e)
        {
			if(_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("saveData", new object[] { _component.ValidationVisible });
			}
        }

        private void _component_ValidationVisibleChanged(object sender, EventArgs e)
        {
			if (_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("showValidation", new object[] {_component.ValidationVisible});
			}
        }

		private void _component_AsyncInvocationCompleted(object sender, AsyncInvocationCompletedEventArgs e)
		{
			if (_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("__asyncInvocationCompleted", new object[] { e.InvocationId, e.Response });
			}
		}

		private void _component_AsyncInvocationError(object sender, AsyncInvocationErrorEventArgs e)
		{
			if (_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("__asyncInvocationError", new object[] { e.InvocationId, e.Error.Message });
			}
		}

        private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
        {
			// navigate to the new URI
			// Bug #2845 even if it is the same URI, we want to navigate rather than refresh, so that scroll position is reset to top
			_webBrowser.Navigate(_component.HtmlPageUrl);
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
