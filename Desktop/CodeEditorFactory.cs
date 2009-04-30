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
        /// Gets or sets the language by file extension (e.g. xml, cs, js, rb).
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets whether the contents of the editor have been modified.
        /// </summary>
        bool Modified { get; set; }

        /// <summary>
        /// Occurs when the value of the <see cref="Modified"/> property changes.
        /// </summary>
        event EventHandler ModifiedChanged;
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
