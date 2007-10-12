#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public abstract class Document
    {
        private object _docID;
        private IWorkspace _workspace;
        private IDesktopWindow _desktopWindow;

        private event EventHandler _closed;

        public Document(object docID, IDesktopWindow desktopWindow)
        {
            _docID = docID;
            _desktopWindow = desktopWindow;
        }

        public void Open()
        {
            LaunchWorkspace(GetComponent(), GetTitle());
        }

        public bool Close()
        {
            if (_workspace != null && _workspace.Close())
            {
                _workspace = null;
                return true;
            }
            return false;
        }

        public event EventHandler Closed
        {
            add { _closed += value; }
            remove { _closed -= value; }
        }

        public void Activate()
        {
            _workspace.Activate();
        }

        protected abstract string GetTitle();

        protected abstract IApplicationComponent GetComponent();

        protected void LaunchWorkspace(IApplicationComponent component, string title)
        {
            try
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    _desktopWindow,
                    component,
                    title,
                    delegate(IApplicationComponent c)
                    {
                        // remove from list of open editors
                        DocumentManager.Remove(_docID);
                        EventsHelper.Fire(_closed, this, EventArgs.Empty);
                    });

                DocumentManager.Set(_docID, this);
            }
            catch (Exception e)
            {
                // could not launch component
                ExceptionHandler.Report(e, _desktopWindow);
            }
        }
    }
}
