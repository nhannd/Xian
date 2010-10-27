#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extends <see cref="IApplicationComponentHost"/> with functionality specific to workspaces.
    /// </summary>
    public interface IWorkspaceHost : IApplicationComponentHost
    {
		/// <summary>
		/// Gets a value indicating whether the workspace in which the component
		/// is hosted is currently the active workspace.
		/// </summary>
		bool IsWorkspaceActive { get; }

		/// <summary>
		/// Occurs when the <see cref="IsWorkspaceActive"/> property changes.
		/// </summary>
    	event EventHandler IsWorkspaceActiveChanged;
    }
}
