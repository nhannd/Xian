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
	/// Defines the public interface to a workspace dialog box.
	/// </summary>
	public interface IWorkspaceDialogBox : IDesktopObject
	{
		/// <summary>
		/// Gets the workspace that owns this dialog box.
		/// </summary>
		IWorkspace Workspace { get; }

		/// <summary>
		/// Gets the hosted component.
		/// </summary>
		object Component { get; }
	}
}
