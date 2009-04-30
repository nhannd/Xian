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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Applets.Scintilla
{
    /// <summary>
    /// Extension point for views onto <see cref="SciCodeEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class SciCodeEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// A code editor based on the ScintillaNET library.
    /// </summary>
    [ExtensionOf(typeof(CodeEditorExtensionPoint))]
    [AssociateView(typeof(SciCodeEditorComponentViewExtensionPoint))]
    public class SciCodeEditorComponent : ApplicationComponent, ICodeEditor
    {
        public class InsertTextEventArgs : EventArgs
        {
            private string _text;
            public InsertTextEventArgs(string text)
            {
                _text = text;
            }

            public string Text
            {
                get { return _text; }
            }
        }

        private string _language;
        private string _text;

        private event EventHandler<InsertTextEventArgs> _insertTextRequested;
        

        /// <summary>
        /// Constructor
        /// </summary>
        public SciCodeEditorComponent()
        {
        }

        #region overrides

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        #region Presentation Model

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    this.Modified = true;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                NotifyPropertyChanged("Language");
            }
        }

        public event EventHandler<InsertTextEventArgs> InsertTextRequested
        {
            add { _insertTextRequested += value; }
            remove { _insertTextRequested -= value; }
        }

        #endregion

        #region ICodeEditor Members

        IApplicationComponent ICodeEditor.GetComponent()
        {
            return this;
        }

        string ICodeEditor.Text
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        void ICodeEditor.InsertText(string text)
        {
            EventsHelper.Fire(_insertTextRequested, this, new InsertTextEventArgs(text));
        }

        string ICodeEditor.Language
        {
            get { return this.Language; }
            set { this.Language = value; }
        }

        bool ICodeEditor.Modified
        {
            get { return this.Modified; }
            set { this.Modified = value; }
        }

        event EventHandler ICodeEditor.ModifiedChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion
    }
}
