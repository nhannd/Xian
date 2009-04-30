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
