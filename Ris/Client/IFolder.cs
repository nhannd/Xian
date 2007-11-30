#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public interface IFolder
    {
        /// <summary>
        /// Gets the ID that identifies the folder
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the text that should be displayed for the folder
        /// </summary>
        string Text { get; }
        
        /// <summary>
        /// Allows the folder to notify that it's text has changed
        /// </summary>
        event EventHandler TextChanged;

        /// <summary>
        /// Gets the iconset that should be displayed for the folder
        /// </summary>
        IconSet IconSet { get; }

        /// <summary>
        /// Allows the folder to nofity that it's icon has changed
        /// </summary>
        event EventHandler IconChanged;

        /// <summary>
        /// Gets the resource resolver that is used to resolve the Icon
        /// </summary>
        IResourceResolver ResourceResolver { get; }

        /// <summary>
        /// Gets the tooltip that should be displayed for the folder
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Allows the folder to notify that it's tooltip has changed
        /// </summary>
        event EventHandler TooltipChanged;

        /// <summary>
        /// Gets the menu model for the context menu that should be displayed when the user right-clicks on the folder
        /// </summary>
        ActionModelNode MenuModel { get; }

        /// <summary>
        /// Gets the open/close state of the current folder
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Indicates if the folder should be initially expanded
        /// </summary>
        bool StartExpanded { get; }

        /// <summary>
        /// Occurs when refresh is about to begin
        /// </summary>
        event EventHandler RefreshBegin;

        /// <summary>
        /// Occurs when refresh is about to finish
        /// </summary>
        event EventHandler RefreshFinish;

        /// <summary>
        /// Asks the folder to refresh its contents.  The implementation may be asynchronous.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Asks the folder to refresh the count of its contents, without actually refreshing the contents.
        /// The implementation may be asynchronous.
        /// </summary>
        void RefreshCount();

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

        /// <summary>
        /// Gets a table of the items that are contained in this folder
        /// </summary>
        ITable ItemsTable { get; }

        /// <summary>
        /// Gets or sets the folder path which sets up the tree structure
        /// </summary>
        Path FolderPath { get; set; }

        /// <summary>
        /// Gets a list of sub folders
        /// </summary>
        IList<IFolder> Subfolders { get; }

        /// <summary>
        /// Add a subfolder
        /// </summary>
        /// <param name="subFolder"></param>
        void AddFolder(IFolder subFolder);

        /// <summary>
        /// Remove a sub folder
        /// </summary>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        bool RemoveFolder(IFolder subFolder);

        /// <summary>
        /// Gets a value indicating whether or not the folder is 'static'.
        /// </summary>
        /// <remarks>
        /// In the context of workflow, folders created via the normal constructor (new Folder(...)) are considered static and are
        /// otherwise they are considered generated if created by Activator.CreateInstance.
        /// </remarks>
        bool IsStatic { get; set; }
    }
}
