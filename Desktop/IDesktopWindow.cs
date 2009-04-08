#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the public interface to a <see cref="DesktopWindow"/>.
    /// </summary>
	/// <remarks>
	/// This interface exists mainly for backward compatibility.  New application
	/// code should use the <see cref="DesktopWindow"/> class.
	/// </remarks>
	public interface IDesktopWindow : IDesktopObject
    {
        /// <summary>
        /// Gets the collection of workspaces associated with this window.
        /// </summary>
        WorkspaceCollection Workspaces { get; }

        /// <summary>
        /// Gets the currently active workspace, or null if there are no workspaces.
        /// </summary>
        Workspace ActiveWorkspace { get; }

        /// <summary>
        /// Gets the collection of shelves associated with this window.
        /// </summary>
        ShelfCollection Shelves { get; }

        /// <summary>
        /// Shows a message box in front of this window.
        /// </summary>
        /// <param name="message">The message to display in the message box.</param>
        /// <param name="buttons">The buttons to display in the message box.</param>
        /// <returns>The result of the user dismissing the message box.</returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);

        /// <summary>
        /// Shows a message box in front of this window.
        /// </summary>
		/// <param name="message">The message to display in the message box.</param>
		/// <param name="title">The title of the message box.</param>
		/// <param name="buttons">The buttons to display in the message box.</param>
		/// <returns>The result of the user dismissing the message box.</returns>
		DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons);

        /// <summary>
        /// Shows a dialog box in front of this window.
        /// </summary>
        /// <param name="component">The <see cref="IApplicationComponent"/> to be hosted in the dialog.</param>
        /// <param name="title">The title of the dialog box.</param>
        /// <returns></returns>
        DialogBoxAction ShowDialogBox(IApplicationComponent component, string title);

        /// <summary>
        /// Shows a dialog box in front of this window.
        /// </summary>
        /// <param name="args">Arguments used to create the dialog box.</param>
        /// <returns>The result of the user dismissing the dialog box.</returns>
        DialogBoxAction ShowDialogBox(DialogBoxCreationArgs args);

		/// <summary>
		/// Shows a 'Save File' common dialog.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
    	FileDialogResult ShowSaveFileDialogBox(FileDialogCreationArgs args);

		/// <summary>
		/// Shows an 'Open File' common dialog.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		FileDialogResult ShowOpenFileDialogBox(FileDialogCreationArgs args);

		/// <summary>
		/// Shows a 'Select Folder' common dialog.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
    	FileDialogResult ShowSelectFolderDialogBox(SelectFolderDialogCreationArgs args);
	}
}
