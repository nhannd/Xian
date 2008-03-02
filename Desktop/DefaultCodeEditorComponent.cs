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
        /// <summary>
        /// In contrast to the usual pattern where the view simply observes changes in the application
        /// component, in this case it is easier to have the view implement an interface such that
        /// the component can control it directly.
        /// </summary>
        public interface IEditorView
        {
            /// <summary>
            /// Inserts the specified text at the current position in the editor.
            /// </summary>
            /// <param name="text"></param>
            void InsertText(string text);
        }

        private string _text;
        private IEditorView _editorView;


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
            if (_editorView != null)
                _editorView.InsertText(text);
        }

        void ICodeEditor.SetLanguage(string language)
        {
            // not supported
        }

        #endregion

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

        public void SetEditorView(IEditorView view)
        {
            _editorView = view;
        }

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
