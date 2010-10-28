#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines additional interface to an application component that acts as a preview page for
	/// selected worklist items.
	/// </summary>
	public interface IPreviewComponent : IApplicationComponent
	{
		/// <summary>
		/// Sets the URL of the preview page to display, and the item(s) that are being previewed.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="items"></param>
		void SetPreviewItems(string url, object[] items);
	}

	/// <summary>
	/// Manages the interaction between a <see cref="FolderExplorerGroupComponent"/>, <see cref="FolderContentsComponent"/>,
	/// and a <see cref="IPreviewComponent"/>, which together form a homepage.
	/// </summary>
	public class HomePageContainer : SplitComponentContainer, ISearchDataHandler
	{
		private readonly FolderExplorerGroupComponent _folderSystemGroup;
		private readonly FolderContentsComponent _folderContentComponent;
		private readonly IPreviewComponent _previewComponent;

		public HomePageContainer(List<IFolderSystem> folderSystems, IPreviewComponent preview)
			: base(Desktop.SplitOrientation.Vertical)
		{
			_folderContentComponent = new FolderContentsComponent();
			_previewComponent = preview;
			_folderSystemGroup = new FolderExplorerGroupComponent(folderSystems, _folderContentComponent);

			// Construct the home page
			SplitComponentContainer contentAndPreview = new SplitComponentContainer(
				new SplitPane("Folder Contents", _folderContentComponent, 0.4f),
				new SplitPane("Content Preview", _previewComponent, 0.6f),
				SplitOrientation.Vertical);

			this.Pane1 = new SplitPane("Folders", _folderSystemGroup, 0.2f);
			this.Pane2 = new SplitPane("Contents", contentAndPreview, 0.8f);
		}

		#region ISearchDataHandler implementation

		public SearchParams SearchParams
		{
			set { _folderSystemGroup.SearchParams = value; }
		}

		public bool SearchEnabled
		{
			get { return _folderSystemGroup.AdvancedSearchEnabled; }
		}

		public event EventHandler SearchEnabledChanged
		{
			add { _folderSystemGroup.SearchEnabledChanged += value; }
			remove { _folderSystemGroup.SearchEnabledChanged -= value; }
		}

		#endregion

		public override void Start()
		{
			_folderSystemGroup.SelectedFolderExplorerChanged += OnSelectedFolderSystemChanged;
			_folderSystemGroup.SelectedFolderChanged += OnSelectedFolderChanged;
			_folderContentComponent.SelectedItemsChanged += SelectedItemsChangedEventHandler;

			base.Start();
		}

		public override void Stop()
		{
			_folderSystemGroup.SelectedFolderExplorerChanged -= OnSelectedFolderSystemChanged;
			_folderSystemGroup.SelectedFolderChanged -= OnSelectedFolderChanged;
			_folderContentComponent.SelectedItemsChanged -= SelectedItemsChangedEventHandler;

			base.Stop();
		}

		private void SelectedItemsChangedEventHandler(object sender, System.EventArgs e)
		{
			// update the preview component url whenever the selected items change,
			// regardless of whether the folder system has changed or not
			// this should help to guarantee that the correct preview page is always displayed
			string url = _folderSystemGroup.SelectedFolderExplorer.SelectedFolder is WorkflowFolder
			             	? _folderSystemGroup.SelectedFolderExplorer.FolderSystem.GetPreviewUrl(
			             			_folderSystemGroup.SelectedFolderExplorer.SelectedFolder,
			             			_folderContentComponent.SelectedItems.Items)
			             	: null;

			_previewComponent.SetPreviewItems(url, _folderContentComponent.SelectedItems.Items);
		}

		private void OnSelectedFolderSystemChanged(object sender, System.EventArgs e)
		{
			_folderContentComponent.FolderSystem = _folderSystemGroup.SelectedFolderExplorer.FolderSystem;
			_folderContentComponent.SelectedFolder = _folderSystemGroup.SelectedFolderExplorer.SelectedFolder;
		}

		private void OnSelectedFolderChanged(object sender, System.EventArgs e)
		{
			_folderContentComponent.SelectedFolder = _folderSystemGroup.SelectedFolderExplorer.SelectedFolder;
		}
	}
}
