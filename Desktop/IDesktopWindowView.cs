#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to a view for a <see cref="DesktopWindow"/> object.
    /// </summary>
    public interface IDesktopWindowView : IDesktopObjectView
    {
        /// <summary>
        /// Creates a view for the specified workspace.
        /// </summary>
        IWorkspaceView CreateWorkspaceView(Workspace workspace);

        /// <summary>
        /// Creates a view for the specified shelf.
        /// </summary>
        IShelfView CreateShelfView(Shelf shelf);

        /// <summary>
        /// Creates a view for the specified dialog box.
        /// </summary>
        IDialogBoxView CreateDialogBoxView(DialogBox dialog);

        /// <summary>
        /// Sets the current menu model.
        /// </summary>
        void SetMenuModel(ActionModelNode model);

        /// <summary>
        /// Sets the current toolbar model.
        /// </summary>
        void SetToolbarModel(ActionModelNode model);

		/// <summary>
		/// Shows a message box in front of this window.
		/// </summary>
		/// <param name="message">The message to display in the message box.</param>
		/// <param name="title">The title of the message box.</param>
		/// <param name="buttons">The buttons to display on the message box.</param>
		/// <returns>The result of the user dismissing the message box.</returns>
		DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons);

		/// <summary>
		/// Sets the alert context.
		/// </summary>
		/// <param name="alertContext"></param>
    	void SetAlertContext(IDesktopAlertContext alertContext);
	
		/// <summary>
		/// Shows an alert notification in front of this window.
		/// </summary>
		void ShowAlert(AlertNotificationArgs args);

		/// <summary>
		/// Shows a 'Save file' dialog in front of this window.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		FileDialogResult ShowSaveFileDialogBox(FileDialogCreationArgs args);

		/// <summary>
		/// Shows an 'Open file' dialog in front of this window.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		FileDialogResult ShowOpenFileDialogBox(FileDialogCreationArgs args);

		/// <summary>
		/// Shows a 'Select folder' dialog in front of this window.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
    	FileDialogResult ShowSelectFolderDialogBox(SelectFolderDialogCreationArgs args);
    }
}
