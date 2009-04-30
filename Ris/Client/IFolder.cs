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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines the interface to a folder.
	/// </summary>
	public interface IFolder
	{
		#region Properties

		/// <summary>
		/// Gets the folder system that owns this folder.
		/// </summary>
		IFolderSystem FolderSystem { get; }

		/// <summary>
		/// Gets the ID that uniquely identifies the folder for use in persisting the folder tree organization.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the text that should be displayed for the folder
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Gets the folder name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the iconset that should be displayed for the folder
		/// </summary>
		IconSet IconSet { get; }

		/// <summary>
		/// Gets the resource resolver that is used to resolve the Icon
		/// </summary>
		IResourceResolver ResourceResolver { get; }

		/// <summary>
		/// Gets the tooltip that should be displayed for the folder
		/// </summary>
		string Tooltip { get; }

		/// <summary>
		/// Gets the open/close state of the current folder
		/// </summary>
		bool IsOpen { get; }

		/// <summary>
		/// Indicates if the folder should be initially expanded
		/// </summary>
		bool StartExpanded { get; }

		/// <summary>
		/// Gets a table of the items that are contained in this folder.
		/// </summary>
		ITable ItemsTable { get; }

		/// <summary>
		/// Gets the total number of items "contained" in this folder, which may be the same
		/// as the number of items displayed in the <see cref="ItemsTable"/>, or may be larger
		/// in the event the table is only showing a subset of the total number of items.
		/// </summary>
		int TotalItemCount { get; }

		/// <summary>
		/// Gets or sets the folder path which determines the location of this folder in the explorer tree.
		/// </summary>
		Path FolderPath { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the folder is 'static'.
		/// </summary>
		bool IsStatic { get; }

		/// <summary>
		/// Gets a value indicating how much time may elapse since <see cref="LastUpdateTime"/> before
		/// this folder is automatically invalidated.
		/// </summary>
		TimeSpan AutoInvalidateInterval { get; }

		/// <summary>
		/// Gets a timestamp indicating the last time that this folder was updated
		/// (which may not be equivalent to the last time that <see cref="Update"/> was called. 
		/// </summary>
		/// <remarks>
		/// The value of this property reflects the last time that the folder was actually updated,
		/// not the last time that <see cref="Update"/> was called (eg. a call to Update has no effect
		/// if the folder has not been invalidated, in which case it should not set this property).
		/// It is the responsiblity of the implementor to set this property appropriately.
		/// </remarks>
		DateTime LastUpdateTime { get; }

		/// <summary>
		/// Gets a value indicating whether the folder is currently in the process of updating.
		/// </summary>
		bool IsUpdating { get; }

		/// <summary>
		/// Gets or sets a value indicating if the folder should be visible.
		/// </summary>
		bool Visible { get; set; }

		#endregion

		#region Events

		/// <summary>
		/// Allows the folder to notify that it's text has changed
		/// </summary>
		event EventHandler TextChanged;

		/// <summary>
		/// Allows the folder to nofity that it's icon has changed
		/// </summary>
		event EventHandler IconChanged;

		/// <summary>
		/// Allows the folder to notify that it's tooltip has changed
		/// </summary>
		event EventHandler TooltipChanged;

		/// <summary>
		/// Occurs to indicate that the entire content of the <see cref="ItemsTable"/> is about to change.
		/// </summary>
		event EventHandler ItemsTableChanging;

		/// <summary>
		/// Occurs to indicate that the entire content of the <see cref="ItemsTable"/> has changed.
		/// </summary>
		event EventHandler ItemsTableChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="TotalItemCount"/> property changes.
		/// </summary>
		event EventHandler TotalItemCountChanged;

		/// <summary>
		/// Occurs before the folder is about to update.
		/// </summary>
		event EventHandler Updating;

		/// <summary>
		/// Occurs after the folder has finished updating.
		/// </summary>
		event EventHandler Updated;

		#endregion

		#region Methods

		/// <summary>
		/// Marks the contents and/or count of the folder as invalid, causing the folder to
		/// update itself with the next call to <see cref="Update"/>.
		/// </summary>
		void Invalidate();

		/// <summary>
		/// Updates the contents and/or count of the folder, if the folder has been invalidated.
		/// Calling this method has no effect if the folder is up-to-date with respect to the 
		/// most recent call to <see cref="Invalidate"/>.
		/// </summary>
		void Update();

		/// <summary>
		/// Opens the folder (i.e. instructs the folder to show its "open" state icon).
		/// </summary>
		void OpenFolder();

		/// <summary>
		/// Closes the folder (i.e. instructs the folder to show its "closed" state icon).
		/// </summary>
		void CloseFolder();

		/// <summary>
		/// Asks the folder if it can accept a drop of the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		/// <returns></returns>
		DragDropKind CanAcceptDrop(object[] items, DragDropKind kind);

		/// <summary>
		/// Instructs the folder to accept the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		DragDropKind AcceptDrop(object[] items, DragDropKind kind);

		/// <summary>
		/// Informs the folder that the specified items were dragged from it.  It is up to the implementation
		/// of the folder to determine the appropriate response (e.g. whether the items should be removed or not).
		/// </summary>
		/// <param name="items"></param>
		/// <param name="result">The result of the drag drop operation</param>
		void DragComplete(object[] items, DragDropKind result);

		#endregion
	}
}
