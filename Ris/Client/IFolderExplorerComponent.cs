#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface to an application component that acts as an explorer for
	/// an underlying <see cref="IFolderSystem"/>.
	/// </summary>
	public interface IFolderExplorerComponent
	{
		/// <summary>
		/// Gets a value indicating whether this folder explorer has already been initialized.
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Instructs the folder explorer to initialize (build the folder system).
		/// </summary>
		void Initialize();

		/// <summary>
		/// Occurs when asynchronous initialization of this folder system has completed.
		/// </summary>
		event EventHandler Initialized;

		/// <summary>
		/// Invalidates all folders.
		/// </summary>
		void InvalidateFolders();

		/// <summary>
		/// Gets the underlying folder system associated with this folder explorer.
		/// </summary>
		IFolderSystem FolderSystem { get; }

		/// <summary>
		/// Gets or sets the currently selected folder.
		/// </summary>
		IFolder SelectedFolder { get; set; }

		/// <summary>
		/// Occurs when the selected folder changes.
		/// </summary>
		event EventHandler SelectedFolderChanged;

		/// <summary>
		/// Executes a search on the underlying folder system.
		/// </summary>
		/// <param name="searchParams"></param>
		void ExecuteSearch(SearchParams searchParams);

		/// <summary>
		/// Launches the advanced search component for the underlying folder system.
		/// </summary>
		void LaunchAdvancedSearchComponent();

		/// <summary>
		/// Gets the application component that displays the content of a folder for this folder system.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetContentComponent();
	}
}