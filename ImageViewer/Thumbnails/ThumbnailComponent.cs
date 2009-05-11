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
using System.Collections.Generic;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	[ExtensionPoint]
	public sealed class ThumbnailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ThumbnailComponentViewExtensionPoint))]
	public partial class ThumbnailComponent : ApplicationComponent
	{
		private static readonly Dictionary<IImageViewer, ImageSetTreeInfo> 
			_viewerTreeInfo = new Dictionary<IImageViewer, ImageSetTreeInfo>();

		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _activeViewer;

		private readonly ImageSetTreeInfo _dummyTreeInfo;
		private ImageSetTreeInfo _currentTreeInfo;

		private readonly BindingList<IGalleryItem> _thumbnails;
		private IEnumerator<IGalleryItem> _loadThumbnailIterator;
		
		public ThumbnailComponent(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
			_dummyTreeInfo = new ImageSetTreeInfo(new ObservableList<IImageSet>(), null);
			_currentTreeInfo = _dummyTreeInfo;
			_thumbnails = new BindingList<IGalleryItem>();
		}

		#region Presentation Model

		public ITree Tree
		{
			get { return _currentTreeInfo.Tree; }
		}

		public ISelection TreeSelection
		{
			get { return _currentTreeInfo.Selection; }
			set { _currentTreeInfo.Selection = value; }
		}

		public BindingList<IGalleryItem> Thumbnails
		{
			get { return _thumbnails; }
		}

		#endregion

		private void UpdateTreeInfo()
		{
			if (_activeViewer == null)
			{
				SetCurrentTreeInfo(_dummyTreeInfo);
			}
			else
			{
				if (!_viewerTreeInfo.ContainsKey(_activeViewer))
				{
					ObservableList<IImageSet> imageSets = _activeViewer.LogicalWorkspace.ImageSets;
					string primaryStudyInstanceUid = GetPrimaryStudyInstanceUid(_activeViewer.StudyTree);
					ImageSetTreeInfo info = new ImageSetTreeInfo(imageSets, primaryStudyInstanceUid);
					_viewerTreeInfo.Add(_activeViewer, info);
				}

				SetCurrentTreeInfo(_viewerTreeInfo[_activeViewer]);
			}
		}

		private void SetCurrentTreeInfo(ImageSetTreeInfo currentTreeInfo)
		{
			if (_currentTreeInfo == currentTreeInfo)
				return;

			if (_currentTreeInfo != null)
			{
				_currentTreeInfo.TreeChanged -= OnTreeChangedInternal;
				_currentTreeInfo.TreeUpdated -= OnTreeUpdatedInternal;
				_currentTreeInfo.SelectionChanged -= OnTreeSelectionChangedInternal;
			}

			_currentTreeInfo = currentTreeInfo;

			if (_currentTreeInfo != null)
			{
				_currentTreeInfo.TreeChanged += OnTreeChangedInternal;
				_currentTreeInfo.TreeUpdated += OnTreeUpdatedInternal;
				_currentTreeInfo.SelectionChanged += OnTreeSelectionChangedInternal;

				OnTreeChangedInternal(null, null);
			}
		}

		private void OnTreeChangedInternal(object sender, EventArgs e)
		{
			NotifyPropertyChanged("Tree");
			OnTreeSelectionChangedInternal(null, null);
		}

		private void OnTreeUpdatedInternal(object sender, EventArgs e)
		{
			//crappy hack - when things are added to the tree dynamically, the view seems to lose the selection.
			NotifyPropertyChanged("TreeSelection");
		}

		private void OnTreeSelectionChangedInternal(object sender, EventArgs e)
		{
			NotifyPropertyChanged("TreeSelection");
			RefreshThumbnails();
		}

		private void OnLoadingPriorsChanged(object sender, EventArgs e)
		{
			UpdateTitle();
		}

		private void UpdateTitle()
		{
			if (_activeViewer != null && _activeViewer.PriorStudyLoader.IsActive)
				base.Host.Title = SR.TitleThumbnailsLoadingPriors;
			else
				base.Host.Title = SR.TitleThumbnails;
		}

		internal static IShelf Launch(IDesktopWindow desktopWindow)
		{
			ThumbnailComponent component = new ThumbnailComponent(desktopWindow);
			Shelf shelf = LaunchAsShelf(
				desktopWindow,
				component,
				SR.TitleThumbnails,
				"Thumbnails",
				ShelfDisplayHint.DockTop | ShelfDisplayHint.DockAutoHide);

			return shelf;
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_desktopWindow.Workspaces.ItemActivationChanged += OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed += OnWorkspaceClosed;
			
			SetImageViewer(_desktopWindow.ActiveWorkspace);
			UpdateTreeInfo();
			
			base.Start();
		}

		public override void Stop()
		{
			SetCurrentTreeInfo(null);
			_dummyTreeInfo.Dispose();

			foreach (ImageSetTreeInfo info in _viewerTreeInfo.Values)
				info.Dispose();

			_viewerTreeInfo.Clear();

			_desktopWindow.Workspaces.ItemActivationChanged -= OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed -= OnWorkspaceClosed;

			SetImageViewer(null);
			ClearThumbnails();

			base.Stop();
		}

		private void OnActiveWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
		{
			SetImageViewer(e.Item);
			UpdateTreeInfo();
		}

		private static void OnWorkspaceClosed(object sender, ClosedItemEventArgs<Workspace> e)
		{
			IImageViewer viewer = CastToImageViewer(e.Item);
			if (viewer != null && _viewerTreeInfo.ContainsKey(viewer))
			{
				ImageSetTreeInfo info = _viewerTreeInfo[viewer];
				_viewerTreeInfo.Remove(viewer);
				info.Dispose();
			}
		}

		private static IImageViewer CastToImageViewer(Workspace workspace)
		{
			IImageViewer viewer = null;
			if (workspace != null)
				viewer = ImageViewerComponent.GetAsImageViewer(workspace);

			return viewer;
		}

		private void SetImageViewer(Workspace workspace)
		{
			IImageViewer viewer = CastToImageViewer(workspace);
			if (viewer == _activeViewer)
				return;

			if (_activeViewer != null)
				_activeViewer.PriorStudyLoader.IsActiveChanged -= OnLoadingPriorsChanged;

			_activeViewer = viewer;

			if (_activeViewer != null)
				_activeViewer.PriorStudyLoader.IsActiveChanged += OnLoadingPriorsChanged;

			UpdateTitle();
		}

		#region Thumbnail Methods

		private void RefreshThumbnails()
		{
			ClearThumbnails();

			if (_activeViewer == null)
				return;

			ImageSetTreeItem imageSetItem = TreeSelection.Item as ImageSetTreeItem;
			if (imageSetItem == null)
				return;

			foreach (IDisplaySet displaySet in imageSetItem.ImageSet.DisplaySets)
			{
				Thumbnail thumbnail = new Thumbnail(displaySet, this.OnThumbnailLoaded);
				_thumbnails.Add(thumbnail);
			}

			//TODO: account for display sets being added or removed?
			_loadThumbnailIterator = _thumbnails.GetEnumerator();
			_loadThumbnailIterator.Reset();

			LoadNextThumbnail();
		}

		private void LoadNextThumbnail()
		{
			if (_loadThumbnailIterator == null)
				return;

			if (_loadThumbnailIterator.MoveNext())
			{
				((Thumbnail)_loadThumbnailIterator.Current).LoadAsync();
			}
			else
			{
				_loadThumbnailIterator.Reset();
				_loadThumbnailIterator = null;
			}
		}

		private void OnThumbnailLoaded(Thumbnail thumbnail)
		{
			int index = _thumbnails.IndexOf(thumbnail);
			if (index >= 0)
				_thumbnails.ResetItem(index);

			LoadNextThumbnail();
		}

		private void ClearThumbnails()
		{
			List<IGalleryItem> thumbnails = new List<IGalleryItem>(_thumbnails);
			foreach (IGalleryItem thumbnail in thumbnails)
			{
				_thumbnails.Remove(thumbnail);
				((IDisposable)thumbnail).Dispose();
			}

			_loadThumbnailIterator = null;
		}

		private static string GetPrimaryStudyInstanceUid(StudyTree studyTree)
		{
			foreach (Patient patient in studyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					return study.StudyInstanceUID;
				}
			}

			return null;
		}

		#endregion
	}
}
