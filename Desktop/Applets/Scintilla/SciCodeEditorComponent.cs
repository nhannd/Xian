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
