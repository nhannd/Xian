using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface to a folder system
	/// </summary>
	public interface IFolderSystem : IDisposable
	{
		#region Properties

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
		IList<IFolder> Folders { get; }

		/// <summary>
		/// Gets the toolset defined for the folders
		/// </summary>
		IToolSet FolderTools { get; }

		/// <summary>
		/// Gets the toolset defined for the items in a folder
		/// </summary>
		IToolSet ItemTools { get; }

		/// <summary>
		/// Gets the Url of the preview when an item is selected
		/// </summary>
		string PreviewUrl { get; }

		#endregion

		#region Events

		/// <summary>
		/// Allows the folder system to notify that its display text has changed
		/// </summary>
		event EventHandler TextChanged;

		/// <summary>
		/// Allows the folder system to notify that its icon has changed
		/// </summary>
		event EventHandler IconChanged;

		#endregion

		#region Methods

		/// <summary>
		/// Invalidates all folders of the specified type so that they
		/// will refresh their content or count.
		/// </summary>
		/// <param name="folderType"></param>
		void InvalidateFolder(Type folderType);

		#endregion

		//TODO: (JR Jun 2008) Get rid of this method - the folder system should subscribe to an event on IFolderExplorerToolContext instead
		void OnSelectedFolderChanged();

		//TODO: (JR Jun 2008) Get rid of this method - the folder system should subscribe to an event on IFolderExplorerToolContext instead
		void OnSelectedItemsChanged();

		//TODO: (JR Jun 2008) Get rid of this method - the folder system should subscribe to an event on IFolderExplorerToolContext instead
		void OnSelectedItemDoubleClicked();
	}
}
