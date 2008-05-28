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

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	public interface IPreviewComponent : IApplicationComponent
	{
		void SetUrl(string url);
	}

	public class HomePageContainer : SplitComponentContainer, ISearchDataHandler
	{
		#region ISearchDataHandler implementation

		public SearchData SearchData
		{
			set { _folderSystemGroup.SearchData = value; }
		}

		#endregion

		private readonly FolderExplorerGroupComponent _folderSystemGroup;
		private readonly FolderContentsComponent _folderContentComponent;
		private readonly IPreviewComponent _previewComponent;

		public HomePageContainer(IExtensionPoint folderExplorerExtensionPoint, IPreviewComponent preview)
			: base(Desktop.SplitOrientation.Vertical)
		{
			_folderContentComponent = new FolderContentsComponent();
			_previewComponent = preview;
			_folderSystemGroup = new FolderExplorerGroupComponent(folderExplorerExtensionPoint, _folderContentComponent);

			// Construct the home page
			SplitComponentContainer contentAndPreview = new SplitComponentContainer(
				new SplitPane("Folder Contents", _folderContentComponent, 0.4f),
				new SplitPane("Content Preview", _previewComponent, 0.6f),
				SplitOrientation.Vertical);

			this.Pane1 = new SplitPane("Folders", _folderSystemGroup, 0.2f);
			this.Pane2 = new SplitPane("Contents", contentAndPreview, 0.8f);
		}

		public override void Start()
		{
			_folderSystemGroup.SelectedFolderSystemChanged += OnSelectedFolderSystemChanged;
			_folderSystemGroup.SelectedFolderChanged += OnSelectedFolderChanged;

			base.Start();
		}

		public override void Stop()
		{
			_folderSystemGroup.SelectedFolderSystemChanged -= OnSelectedFolderSystemChanged;
			_folderSystemGroup.SelectedFolderChanged -= OnSelectedFolderChanged;

			base.Stop();
		}

		public FolderContentsComponent ContentsComponent
		{
			get { return _folderContentComponent; }
		}

		public IPreviewComponent PreviewComponent
		{
			get { return _previewComponent; }
		}

		private void OnSelectedFolderSystemChanged(object sender, System.EventArgs e)
		{
			_folderContentComponent.FolderSystem = _folderSystemGroup.SelectedFolderSystem;
			_previewComponent.SetUrl(_folderSystemGroup.SelectedFolderSystem.PreviewUrl);

			_folderContentComponent.SelectedFolder = _folderSystemGroup.SelectedFolder;

			if (_folderContentComponent.SelectedFolder != null)
				_folderContentComponent.SelectedFolder.Refresh();
		}

		private void OnSelectedFolderChanged(object sender, System.EventArgs e)
		{
			_folderContentComponent.SelectedFolder = _folderSystemGroup.SelectedFolder;

			if (_folderContentComponent.SelectedFolder != null)
				_folderContentComponent.SelectedFolder.Refresh();
		}
	}
}
