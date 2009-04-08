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

using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Delegate used to determine if a double-click handler is enabled.
	/// </summary>
	/// <returns></returns>
	public delegate bool DoubleClickHandlerEnablementDelegate();

	/// <summary>
	/// Defines a base tool context for tools that operate on workflow items.
	/// </summary>
	public interface IWorkflowItemToolContext : IToolContext
	{
		/// <summary>
		/// Gets the desktop window.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the currently selected folder.
		/// </summary>
		IFolder SelectedFolder { get; }

		/// <summary>
		/// Gets the current selection of items.
		/// </summary>
		ISelection Selection { get; }

		/// <summary>
		/// Occurs when <see cref="Selection"/> changes.
		/// </summary>
		event EventHandler SelectionChanged;

		/// <summary>
		/// Invalidates all folders. Use this method judiciously,
		/// as invalidating all folders will increase load on the system.
		/// </summary>
		void InvalidateFolders();

		/// <summary>
		/// Invalidates the currently selected folder.
		/// </summary>
		void InvalidateSelectedFolder();

		/// <summary>
		/// Invalidates all folders of the specified class.
		/// </summary>
		/// <param name="folderClass"></param>
		void InvalidateFolders(Type folderClass);


		/// <summary>
		/// Allows the tool to register itself as a double-click handler for items,
		/// regardless of which folder they are in.
		/// </summary>
		/// <remarks>
		/// More than one tool may register a double-click handler, but at most one will receive
		/// the notification.  The first handler whose enablement function returns true will receive the call. 
		/// </remarks>
		/// <param name="clickAction"></param>
		void RegisterDoubleClickHandler(IClickAction clickAction);

		/// <summary>
		/// Allows the tool to un-register itself as a double-click handler for items,
		/// regardless of which folder they are in.
		/// </summary>
		/// <remarks>
		/// If the tool is not registered as a double-click handler, nothing will happen.
		/// </remarks>
		void UnregisterDoubleClickHandler(IClickAction clickAction);

		/// <summary>
		/// Allows the tool to register a workflow service with the folder system.  When the selection changes,
		/// the folder system queries the operation enablement of all registered workflow services, and the
		/// results are available to the tool by calling <see cref="GetOperationEnablement(string)"/>.
		/// </summary>
		/// <param name="serviceContract"></param>
		void RegisterWorkflowService(Type serviceContract);

		/// <summary>
		/// Gets a value indicating whether the specified operation is enabled for the current items selection.
		/// </summary>
		/// <param name="serviceContract"></param>
		/// <param name="operationClass"></param>
		/// <returns></returns>
		bool GetOperationEnablement(Type serviceContract, string operationClass);

		/// <summary>
		/// Gets a value indicating whether the specified operation is enabled for the current items selection.
		/// </summary>
		/// <param name="operationClass"></param>
		/// <returns></returns>
		bool GetOperationEnablement(string operationClass);
	}

	/// <summary>
	/// Defines a base tool context for tools that operate on workflow items.
	/// </summary>
	public interface IWorkflowItemToolContext<TItem> : IWorkflowItemToolContext
	{
		/// <summary>
		/// Gets the currently selection items as a strongly typed collection.
		/// </summary>
		ICollection<TItem> SelectedItems { get; }

		/// <summary>
		/// Allows the tool to register a drag-drop handler on the specified folder class.
		/// </summary>
		/// <param name="folderClass"></param>
		/// <param name="dropHandler"></param>
		void RegisterDropHandler(Type folderClass, IDropHandler<TItem> dropHandler);

		/// <summary>
		/// Allows the tool to un-register a drag-drop handler on the specified folder class.
		/// </summary>
		/// <param name="folderClass"></param>
		/// <param name="dropHandler"></param>
		void UnregisterDropHandler(Type folderClass, IDropHandler<TItem> dropHandler);
	}
}
