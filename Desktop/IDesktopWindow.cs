#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		/// Shows a desktop alert.
		/// </summary>
		/// <param name="level">The alert level.</param>
		/// <param name="message">The message to display.</param>
    	void ShowAlert(AlertLevel level, string message);

		/// <summary>
		/// Shows a desktop alert.
		/// </summary>
		/// <param name="level">The alert level.</param>
		/// <param name="message">The message to display.</param>
		/// <param name="linkText">The link text to display.</param>
		/// <param name="linkAction">The link action to display.</param>
		void ShowAlert(AlertLevel level, string message, string linkText, System.Action<DesktopWindow> linkAction);

		/// <summary>
		/// Shows a desktop alert.
		/// </summary>
		/// <param name="args"></param>
    	void ShowAlert(AlertNotificationArgs args);

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
