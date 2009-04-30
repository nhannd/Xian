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
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class HtmlComponentControl : CustomUserControl
    {
        private IApplicationComponent _component;
        private ActiveTemplate _template;
        private string _cachedHtml;


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
