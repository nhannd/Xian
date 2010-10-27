#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines the public interface to a <see cref="Workspace"/>.
	/// </summary>
	/// <remarks>
	/// This interface exists mainly for backward compatibility.  New application
	/// code should use the <see cref="Workspace"/> class.
	/// </remarks>
	public interface IWorkspace : IDesktopObject
	{
		/// <summary>
		/// Gets the desktop window that owns this workspace.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the hosted component.
		/// </summary>
		object Component { get; }

		/// <summary>
		/// Gets the command history associated with this workspace.
		/// </summary>
		CommandHistory CommandHistory { get; }

		/// <summary>
		/// Gets a value indicating whether this workspace can be closed directly by the user.
		/// </summary>
		bool UserClosable { get; }

		/// <summary>
		/// Shows a dialog box in front of this workspace.
		/// </summary>
		/// <param name="args">Arguments used to create the dialog box.</param>
		/// <returns>The newly created dialog box object.</returns>
		WorkspaceDialogBox ShowDialogBox(DialogBoxCreationArgs args);
	}
}
