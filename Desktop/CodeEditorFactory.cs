using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an interface to an editor that is specialized for editing source code.
    /// </summary>
    public interface ICodeEditor
    {
        /// <summary>
        /// Gets the application component that implements the code editor.
        /// </summary>
        /// <returns></returns>
        IApplicationComponent GetComponent();

        /// <summary>
        /// Gets or sets the text that appears in the editor.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Inserts the specified text into the editor at the current caret location.
        /// </summary>
        /// <param name="text"></param>
        void InsertText(string text);

        /// <summary>
        /// Sets the language by file extension (e.g. xml, cs, js, rb).
        /// </summary>
        /// <param name="language"></param>
        void SetLanguage(string language);
    }

    /// <summary>
    /// Defines an extension point for an editor that is specialized for editing source code.
    /// </summary>
    [ExtensionPoint]
    public class CodeEditorExtensionPoint : ExtensionPoint<ICodeEditor>
    {
    }


    /// <summary>
    /// Factory for creating instances of <see cref="ICodeEditor"/>.
    /// </summary>
    public static class CodeEditorFactory
    {
        /// <summary>
        /// Creates an returns an instance of <see cref="ICodeEditor"/>.
        /// If an extension of <see cref="CodeEditorExtensionPoint"/> exists, an instance of this extension
        /// will be returned.  Otherwise, a default implementation will be returned.
        /// </summary>
        /// <returns></returns>
        public static ICodeEditor CreateCodeEditor()
        {
            try
            {
                return (ICodeEditor)new CodeEditorExtensionPoint().CreateExtension();
            }
            catch (NotSupportedException)
            {
                return new DefaultCodeEditorComponent();
            }
        }
    }
}
