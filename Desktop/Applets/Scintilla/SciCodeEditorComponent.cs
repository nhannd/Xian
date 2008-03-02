using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

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
        /// <summary>
        /// In contrast to the usual pattern where the view simply observes changes in the application
        /// component, in this case it is easier to have the view implement an interface such that
        /// the component can control it directly.
        /// </summary>
        public interface IEditorView
        {
            /// <summary>
            /// Gets or sets the text that appears in the editor.
            /// </summary>
            string Text { get; set; }

            /// <summary>
            /// Inserts the specified text at the current position in the editor.
            /// </summary>
            /// <param name="text"></param>
            void InsertText(string text);
        }

        private string _language;

        private IEditorView _editorView;

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

        public void SetEditorView(IEditorView view)
        {
            _editorView = view;
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

        #endregion

        #region ICodeEditor Members

        IApplicationComponent ICodeEditor.GetComponent()
        {
            return this;
        }

        string ICodeEditor.Text
        {
            get { return _editorView.Text; }
            set { _editorView.Text = value; }
        }

        void ICodeEditor.InsertText(string text)
        {
            _editorView.InsertText(text);
        }

        void ICodeEditor.SetLanguage(string language)
        {
            this.Language = language;
        }

        #endregion
    }
}
