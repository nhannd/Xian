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
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface to a folder system
	/// </summary>
	public interface IFolderSystem : IDisposable
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating whether initialization of this folder system can be deferred.
		/// </summary>
		/// <remarks>
		/// If the folder system displays a status message, such as a total item count, in the
		/// title bar of the explorer, then deferring initialization is probably not a good idea,
		/// since the title bar will remain empty until the folder system is initialized.
		/// </remarks>
		bool LazyInitialize { get; }

		/// <summary>
		/// Gets the desktop window of the folder explorer that is hosting this folder system.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the ID that identifies the folder system
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the text that should be displayed for the folder system
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Gets the iconset that should be displayed for the folder system
		/// </summary>
		IconSet TitleIcon { get; }

		/// <summary>
		/// Gets the resource resolver that is used to resolve the title icon
		/// </summary>
		IResourceResolver ResourceResolver { get; }

		/// <summary>
		/// Gets the list of folders that belong to this folder system
		/// </summary>
		ObservableList<IFolder> Folders { get; }

		/// <summary>
		/// Gets the toolset defined for the folders
		/// </summary>
		IToolSet FolderTools { get; }

		/// <summary>
		/// Gets the toolset defined for the items in a folder
		/// </summary>
		IToolSet ItemTools { get; }

		/// <summary>
		/// Gets the URL of the preview page as a function of the specified folder and items.
		/// </summary>
		string GetPreviewUrl(IFolder folder, object[] items);

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the <see cref="Title"/> property changes.
		/// </summary>
		event EventHandler TitleChanged;

		/// <summary>
		/// Occurs when the <see cref="TitleIcon"/> property changes.
		/// </summary>
		event EventHandler TitleIconChanged;

		/// <summary>
		/// Occurs to indicate that the entire folder system should be rebuilt.
		/// </summary>
		/// <remarks>
		/// This is distinct from incremental changes to the <see cref="Folders"/> collection,
		/// which can be observed via the <see cref="ObservableList{TItem}.ItemAdded"/> and
		/// <see cref="ObservableList{TItem}.ItemRemoved"/> events.
		/// </remarks>
		event EventHandler FoldersChanged;

		/// <summary>
		/// Occurs when one or more folders in the system have been invalidated.
		/// </summary>
		event EventHandler FoldersInvalidated;

		#endregion

		#region Methods

		/// <summary>
		/// Initializes the folder system with a context.
		/// </summary>
		/// <param name="context"></param>
		void SetContext(IFolderSystemContext context);

		/// <summary>
		/// Initialize the folder system.
		/// </summary>
		/// <remarks>
		/// This method will be called after <see cref="SetContext"/> has been called. 
		/// </remarks>
		void Initialize();

		/// <summary>
		/// Invalidates all folders. Use this method judiciously,
		/// as invalidating all folders will increase load on the system.
		/// </summary>
		void InvalidateFolders();

		/// <summary>
		/// Invalidates all folders of the specified class in this folder system.
		/// </summary>
		void InvalidateFolders(Type folderClass);

		/// <summary>
		/// Invalidates the specified folder.
		/// </summary>
		/// <param name="folder"></param>
		void InvalidateFolder(IFolder folder);


		#endregion

		/// <summary>
		/// Gets a value indicating whether this folder system supports searching.
		/// </summary>
		bool SearchEnabled { get; }

		/// <summary>
		/// Gets a value indicating whether this folder system supports advanced searching.
		/// </summary>
		bool AdvancedSearchEnabled { get; }

		/// <summary>
		/// Get a message to describe the type of search performed.
		/// </summary>
		string SearchMessage { get; }

		/// <summary>
		/// Performs a search, if enabled.
		/// </summary>
		/// <param name="params"></param>
		void ExecuteSearch(SearchParams @params);

		/// <summary>
		/// Returns a <see cref="SearchParams"/> object appropriate to this folder system
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		SearchParams CreateSearchParams(string search);

		/// <summary>
		/// Launches a <see cref="SearchComponentBase"/> appropriate to this folder system
		/// </summary>
		void LaunchSearchComponent();

		/// <summary>
		/// Indicates the type of <see cref="SearchComponentBase"/> appropriate to this folder system
		/// </summary>
		Type SearchComponentType { get; }
	}
}
