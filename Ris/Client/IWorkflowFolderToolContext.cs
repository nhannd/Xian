#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines interface to context for tools that operate on workflow folders.
	/// </summary>
    public interface IWorkflowFolderToolContext : IToolContext
    {
		/// <summary>
		/// Gets the set of folders in the folder system.
		/// </summary>
        IEnumerable<IFolder> Folders { get; }

		/// <summary>
		/// Gets the currently selected folder, or null if no folder is selected.
		/// </summary>
        IFolder SelectedFolder { get; }

		/// <summary>
		/// Occurs when <see cref="SelectedFolder"/> changes.
		/// </summary>
        event EventHandler SelectedFolderChanged;

		/// <summary>
		/// Gets the desktop window.
		/// </summary>
        IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Invalidates all folders in the folder system.
		/// </summary>
		void InvalidateFolders();

		/// <summary>
		/// Invalidates all folders of the specified class in the folder system.
		/// </summary>
		void InvalidateFolders(Type folderClass);
    }
}
