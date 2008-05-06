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
		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _activeViewer;
		private static readonly Dictionary<IImageViewer, ISelection> _selectionCache=
			new Dictionary<IImageViewer, ISelection>();

		private readonly Tree<IImageSetTreeItem> _imageSetTree;
		private ISelection _imageSetTreeSelection;

		private readonly BindingList<IGalleryItem> _displaySetGalleryItems;
		private readonly DisplaySetGalleryItemLoader _galleryItemLoader;
		
		public ThumbnailComponent(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
			_imageSetTree = new Tree<IImageSetTreeItem>(new ImageSetTreeItemBinding());
			_galleryItemLoader = new DisplaySetGalleryItemLoader(this);
			_displaySetGalleryItems = new BindingList<IGalleryItem>();
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
				if (_imageSetTreeSelection == value)
					return;

				_imageSetTreeSelection = value ?? new Selection();
				NotifyPropertyChanged("ImageSetTreeSelection");

				RefreshThumbnails();
			}
		}

		public BindingList<IGalleryItem> DisplaySetGalleryItems
		{
			get { return _displaySetGalleryItems; }
		}

		#endregion

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_desktopWindow.Workspaces.ItemActivationChanged += OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed += OnWorkspaceClosed;

			_galleryItemLoader.Start();

			SetImageViewer(_desktopWindow.ActiveWorkspace);

			base.Start();
		}

		public override void Stop()
		{
			_desktopWindow.Workspaces.ItemActivationChanged -= OnActiveWorkspaceChanged;
			_desktopWindow.Workspaces.ItemClosed -= OnWorkspaceClosed;

			SetImageViewer(null);
			_galleryItemLoader.Stop(true);

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
				_selectionCache.Remove(viewer);
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
				_selectionCache[_activeViewer] = this.ImageSetTreeSelection;

			_activeViewer = viewer;

			SynchronizationContext.Current.Post
				(
					delegate 
					{ 
						//this is a bit cheap, but we do it because when a new ImageViewerComponent is
						//created, it doesn't have any ImageSets or DisplaySets yet.
						if (_activeViewer == viewer && _galleryItemLoader.Active)
							RefreshTree();
					}
				, null);
		}

		#region Thumbnail Methods

		private void RefreshThumbnails()
		{
			List<IGalleryItem> items = new List<IGalleryItem>(_displaySetGalleryItems);
			items.ForEach(
				delegate (IGalleryItem item)
				{
					_displaySetGalleryItems.Remove(item);
					((IDisposable)item).Dispose();
				});

			if (_activeViewer == null)
				return;

			if (_imageSetTree.Items.Count == 0 || 
				this.ImageSetTreeSelection.Item == null || 
				this.ImageSetTreeSelection.Item is PatientTreeItem)
			{
				return;
			}

			ImageSetTreeItem imageSetItem = (ImageSetTreeItem)this.ImageSetTreeSelection.Item;
			foreach (IDisplaySet displaySet in imageSetItem.ImageSet.DisplaySets)
			{
				DisplaySetGalleryItem galleryItem = new DisplaySetGalleryItem(displaySet);
				_displaySetGalleryItems.Add(galleryItem);

				_galleryItemLoader.Enqueue(galleryItem);
			}
		}

		private void RefreshGalleryItem(IGalleryItem item)
		{
			int index = _displaySetGalleryItems.IndexOf(item);
			if (index >= 0)
				_displaySetGalleryItems.ResetItem(index);
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
				ISelection initialSelection = GetInitialSelection(tree);

				_imageSetTree.Items.AddRange(tree);
				this.ImageSetTreeSelection = initialSelection;
			}
		}

		private List<IImageSetTreeItem> BuildTree()
		{
			List<IImageSetTreeItem> tree = new List<IImageSetTreeItem>();

			foreach (IImageSet imageSet in _activeViewer.LogicalWorkspace.ImageSets)
			{
				PatientTreeItem addToItem = tree.Find(
					delegate(IImageSetTreeItem item)
						{
							return ((PatientTreeItem)item).PatientInfo == imageSet.PatientInfo;
						}) as PatientTreeItem;

				if (addToItem == null)
				{
					addToItem = new	PatientTreeItem(imageSet.PatientInfo);
					tree.Add(addToItem);
				}

				addToItem.ImageSetSubTree.Items.Add(new ImageSetTreeItem(imageSet));
			}

			return tree;
		}

		private ISelection GetInitialSelection(IList<IImageSetTreeItem> tree)
		{
			PatientTreeItem lastPatientItem = null;
			ImageSetTreeItem lastImageSetItem = null;

			if (_selectionCache.ContainsKey(_activeViewer))
			{
				ISelection lastSelection = _selectionCache[_activeViewer];
				lastPatientItem = lastSelection.Item as PatientTreeItem;
				lastImageSetItem = lastSelection.Item as ImageSetTreeItem;
			}

			if (lastPatientItem == null && lastImageSetItem == null)
				lastImageSetItem = ((PatientTreeItem)tree[0]).ImageSetSubTree.Items[0] as ImageSetTreeItem;

			ISelection newSelection = null;

			foreach (PatientTreeItem patientItem in tree)
			{
				if (lastPatientItem != null && lastPatientItem.PatientInfo == patientItem.PatientInfo)
				{
					patientItem.IsIntiallyExpanded = true;
					newSelection = new Selection(patientItem);
				}
				else if (lastImageSetItem != null)
				{
					foreach (ImageSetTreeItem imageSetItem in patientItem.ImageSetSubTree.Items)
					{
						if (lastImageSetItem.ImageSet == imageSetItem.ImageSet)
						{
							patientItem.IsIntiallyExpanded = true;
							newSelection = new Selection(imageSetItem);
							break;
						}
					}
				}

				if (newSelection != null)
					break;
			}

			return newSelection;
		}

		#endregion
	}
}
