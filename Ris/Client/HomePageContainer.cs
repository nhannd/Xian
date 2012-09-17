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
		private readonly StackedComponentContainer _contentArea;

		// default content component consists of the FolderContents and a preview pane
		private readonly SplitComponentContainer _defaultContentComponent;
		private readonly FolderContentsComponent _folderContentComponent;
		private readonly IPreviewComponent _previewComponent;

		public HomePageContainer(List<IFolderSystem> folderSystems, IPreviewComponent preview)
			: base(SplitOrientation.Vertical)
		{
			_folderContentComponent = new FolderContentsComponent();
			_folderSystemGroup = new FolderExplorerGroupComponent(folderSystems, _folderContentComponent);

			// Construct the default content view
			_previewComponent = preview;
			_defaultContentComponent = new SplitComponentContainer(
				new SplitPane("FolderItems", _folderContentComponent, 0.4f),
				new SplitPane("ItemPreview", _previewComponent, 0.6f),
				SplitOrientation.Vertical);

			_contentArea = new StackedComponentContainer();
			_contentArea.Show(_defaultContentComponent);

			this.Pane1 = new SplitPane("Folders", _folderSystemGroup, 0.2f);
			this.Pane2 = new SplitPane("Contents", _contentArea, 0.8f);
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

		private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
		{
			// update the preview component url whenever the selected items change,
			// regardless of whether the folder system has changed or not
			// this should help to guarantee that the correct preview page is always displayed
			var selectedExplorer = _folderSystemGroup.SelectedFolderExplorer;
			var url = selectedExplorer.SelectedFolder is WorkflowFolder
							? selectedExplorer.FolderSystem.GetPreviewUrl(selectedExplorer.SelectedFolder, _folderContentComponent.SelectedItems.Items)
			             	: null;

			_previewComponent.SetPreviewItems(url, _folderContentComponent.SelectedItems.Items);

			if(selectedExplorer.SelectedFolder != null)
			{
				AuditHelper.FolderItemPreviewed(selectedExplorer.SelectedFolder, _folderContentComponent.SelectedItems.Items);
			}
		}

		private void OnSelectedFolderSystemChanged(object sender, EventArgs e)
		{
			var selectedFolderExplorer = _folderSystemGroup.SelectedFolderExplorer;
			var customContentComponent = selectedFolderExplorer.GetContentComponent();
			if(customContentComponent != null)
			{
				// this folder system has a custom content component, so display it
				_contentArea.Show(customContentComponent);
			}
			else
			{
				// this folder system uses the default folder content component, so we just
				// need to update it to reflect current selection
				_folderContentComponent.FolderSystem = selectedFolderExplorer.FolderSystem;
				_folderContentComponent.SelectedFolder = selectedFolderExplorer.SelectedFolder;
				_contentArea.Show(_defaultContentComponent);
			}
		}

		private void OnSelectedFolderChanged(object sender, EventArgs e)
		{
			var selectedFolderExplorer = _folderSystemGroup.SelectedFolderExplorer;
			var customContentComponent = selectedFolderExplorer.GetContentComponent();
			if (customContentComponent == null)
			{
				_folderContentComponent.SelectedFolder = _folderSystemGroup.SelectedFolderExplorer.SelectedFolder;
			}
		}
	}
}
