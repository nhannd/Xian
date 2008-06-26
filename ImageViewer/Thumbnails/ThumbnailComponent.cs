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

using System;
using System.Collections.Generic;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Trees;
using System.Threading;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	[ExtensionPoint]
	public sealed class ThumbnailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ThumbnailComponentViewExtensionPoint))]
	public partial class ThumbnailComponent : ApplicationComponent
	{
		private static readonly Dictionary<IImageViewer, ISelection> _imageSetTreeSelectionCache =
			new Dictionary<IImageViewer, ISelection>();

		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _activeViewer;

		private readonly Tree<IImageSetTreeItem> _imageSetTree;
		private ISelection _imageSetTreeSelection;

		private readonly BindingList<IGalleryItem> _thumbnails;
		private IEnumerator<IGalleryItem> _loadThumbnailIterator;
		
		public ThumbnailComponent(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
			_thumbnails = new BindingList<IGalleryItem>();
			_imageSetTree = new Tree<IImageSetTreeItem>(new ImageSetTreeItemBinding());
			_imageSetTreeSelection = new Selection();
		}

		#region Presentation Model

		public ITree ImageSetTree
		{
			get { return _imageSetTree; }
		}

		public ISelection ImageSetTreeSelection
		{
			get { return _imageSetTreeSelection; }
			set
			{
				value = value ?? new Selection();

				if (_imageSetTreeSelection.Equals(value))
					return;

				_imageSetTreeSelection = value;
				NotifyPropertyChanged("ImageSetTreeSelection");

				RefreshThumbnails();
			}
		}

		public BindingList<IGalleryItem> Thumbnails
		{
			get { return _thumbnails; }
		}

		#endregion

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_desktopWindow.Workspaces.ItemActivationChanged += OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed += OnWorkspaceClosed;

			SetImageViewer(_desktopWindow.ActiveWorkspace);

			base.Start();
		}

		public override void Stop()
		{
			_desktopWindow.Workspaces.ItemActivationChanged -= OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed -= OnWorkspaceClosed;

			_activeViewer = null;
			ClearThumbnails();

			base.Stop();
		}

		private void OnActiveWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
		{
			SetImageViewer(e.Item);
		}

		private static void OnWorkspaceClosed(object sender, ClosedItemEventArgs<Workspace> e)
		{
			IImageViewer viewer = CastToImageViewer(e.Item);
			if (viewer != null)
				_imageSetTreeSelectionCache.Remove(viewer);
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

			if (_activeViewer == viewer)
				return;

			if (_activeViewer != null)
				_imageSetTreeSelectionCache[_activeViewer] = this.ImageSetTreeSelection;

			_activeViewer = viewer;

			SynchronizationContext.Current.Post
				(
					delegate
					{
						//this is a bit cheap, but we do it because when a new ImageViewerComponent is
						//created, it doesn't have any ImageSets or DisplaySets yet.
						if (_activeViewer == viewer && this.IsStarted)
							RefreshTree();

					}, null);
		}

		#region Thumbnail Methods

		private void RefreshThumbnails()
		{
			ClearThumbnails();

			if (_activeViewer == null)
				return;

			ImageSetTreeItem imageSetItem = ImageSetTreeSelection.Item as ImageSetTreeItem;
			if (imageSetItem == null)
				return;

			foreach (IDisplaySet displaySet in imageSetItem.ImageSet.DisplaySets)
			{
				Thumbnail thumbnail = new Thumbnail(displaySet, this.OnThumbnailLoaded);
				_thumbnails.Add(thumbnail);
			}

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
			thumbnails.ForEach(
				delegate(IGalleryItem thumbnail)
					{
						_thumbnails.Remove(thumbnail);
						((IDisposable)thumbnail).Dispose();
					});

			_loadThumbnailIterator = null;
		}

		#endregion

		#region ImageSet Tree Methods

		private void RefreshTree()
		{
			_imageSetTree.Items.Clear();

			ImageSetTreeSelection = null;

			if (_activeViewer != null && _activeViewer.LogicalWorkspace.ImageSets.Count > 0)
			{
				List<IImageSetTreeItem> tree = BuildTree();
				ISelection initialSelection;
				InitializeTree(tree, out initialSelection);

				_imageSetTree.Items.AddRange(tree);
				this.ImageSetTreeSelection = initialSelection;
			}
		}

		private List<IImageSetTreeItem> BuildTree()
		{
			List<IImageSetTreeItem> tree = new List<IImageSetTreeItem>();

			foreach (IImageSet imageSet in _activeViewer.LogicalWorkspace.ImageSets)
			{
				PatientTreeItem existingPatientItem = CollectionUtils.SelectFirst(tree,
								   delegate(IImageSetTreeItem item)
								   {
									   return ((PatientTreeItem)item).PatientInfo == imageSet.PatientInfo;
								   }) as PatientTreeItem;

				if (existingPatientItem == null)
				{
					existingPatientItem = new PatientTreeItem(imageSet.PatientInfo);
					tree.Add(existingPatientItem);
				}

				existingPatientItem.ImageSetSubTree.Items.Add(new ImageSetTreeItem(imageSet));
			}

			return tree;
		}

		private void InitializeTree(IList<IImageSetTreeItem> tree, out ISelection initialSelection)
		{
			initialSelection = null;

			PatientTreeItem lastPatientItem = null;
			ImageSetTreeItem lastImageSetItem = null;

			if (_imageSetTreeSelectionCache.ContainsKey(_activeViewer))
			{
				ISelection lastSelection = _imageSetTreeSelectionCache[_activeViewer];
				lastPatientItem = lastSelection.Item as PatientTreeItem;
				lastImageSetItem = lastSelection.Item as ImageSetTreeItem;
			}

			if (lastPatientItem == null && lastImageSetItem == null)
				lastImageSetItem = ((PatientTreeItem)tree[0]).ImageSetSubTree.Items[0] as ImageSetTreeItem;

			foreach (PatientTreeItem patientItem in tree)
			{
				if (lastPatientItem != null && lastPatientItem.PatientInfo == patientItem.PatientInfo)
				{
					patientItem.IsIntiallyExpanded = true;
					initialSelection = new Selection(patientItem);
				}
				else if (lastImageSetItem != null)
				{
					foreach (ImageSetTreeItem imageSetItem in patientItem.ImageSetSubTree.Items)
					{
						if (lastImageSetItem.ImageSet == imageSetItem.ImageSet)
						{
							patientItem.IsIntiallyExpanded = true;
							initialSelection = new Selection(imageSetItem);
							break;
						}
					}
				}

				if (initialSelection != null)
					break;
			}
		}

		#endregion
	}
}
