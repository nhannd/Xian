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
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.ExtensionBrowser.View.WinForms
{
    /// <summary>
    /// WinForms implemenation of a view onto <see cref="ExtensionBrowserComponent"/>.  This class
    /// delegates all of the real work to the <see cref="ExtensionBrowserControl"/> class.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ExtensionBrowserComponentViewExtensionPoint))]
    public class ExtensionBrowserView : WinFormsView, IApplicationComponentView
    {
        private ExtensionBrowserComponent _browser;
        private ExtensionBrowserControl _browserControl;
       
        public ExtensionBrowserView()
        {
        }

        /// <summary>
        /// Implementation of <see cref="IApplicationComponentView.SetComponent"/>
        /// </summary>
        /// <param name="component"></param>
        public void SetComponent(IApplicationComponent component)
        {
            _browser = (ExtensionBrowserComponent)component;
        }

        /// <summary>
        /// Implementation of <see cref="IView.GuiElement"/>.  Gets the WinForms
        /// element that provides the UI for this view.
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_browserControl == null)
                {
                    _browserControl = new ExtensionBrowserControl(_browser);
                }
                return _browserControl;
            }
        }
    }
}
