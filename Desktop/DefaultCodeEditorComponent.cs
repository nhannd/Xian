using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{


    /// <summary>
    /// Extension point for views onto <see cref="DefaultCodeEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DefaultCodeEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// A default code editor class component class.  This is a no frills editor and is typically
    /// used only when a better code editor does not exist in the installed plugin base.
    /// </summary>
    [AssociateView(typeof(DefaultCodeEditorComponentViewExtensionPoint))]
    public class DefaultCodeEditorComponent : ApplicationComponent, ICodeEditor
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

        private string _text;
        private event EventHandler<InsertTextEventArgs> _insertTextRequested;


        /// <summary>
        /// Constructor
        /// </summary>
        internal DefaultCodeEditorComponent()
        {
        }

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
            get { return null; }
            set { /* not supported */ }
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

        #region overrides

        /// <summary>
        /// Starts the component.
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        #region Presentation Model

        /// <summary>
        /// Notifies the view that it should insert the specified text at the current location.
        /// </summary>
        public event EventHandler<InsertTextEventArgs> InsertTextRequested
        {
            add { _insertTextRequested += value; }
            remove { _insertTextRequested -= value; }
        }

        /// <summary>
        /// Gets or sets the text that is displayed in the editor.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                this.Modified = true;
                NotifyPropertyChanged("Text");
            }
        }

        #endregion

    }
}
