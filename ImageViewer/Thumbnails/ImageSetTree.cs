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

using System.Collections.Generic;
using ClearCanvas.Desktop.Trees;
using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System.Collections.ObjectModel;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	public partial class ThumbnailComponent
	{
		#region ImageSet Tree Info

		private class ImageSetTreeInfo : IDisposable
		{
			#region Private Fields

			private readonly string _primaryStudyInstanceUid;
			private readonly ImageSetGroups _imageSetGroups;
			private readonly ImageSetTreeGroupItem _internalTree;
			private ImageSetTreeGroupItem _internalTreeRoot;
			private ISelection _selection;

			#endregion

			public ImageSetTreeInfo(ObservableList<IImageSet> imageSets, string primaryStudyInstanceUid)
			{
				_primaryStudyInstanceUid = primaryStudyInstanceUid;
				_imageSetGroups = new ImageSetGroups(imageSets);
				_internalTree = new ImageSetTreeGroupItem(_imageSetGroups.Root, new StudyDateComparer());
				UpdateInternalTreeRoot();
				_internalTree.Updated += OnInternalTreeUpdated;
			}

			#region Public Properties / Events

			public ITree Tree
			{
				get { return _internalTreeRoot.Tree; }	
			}

			public event EventHandler TreeChanged;
			public event EventHandler TreeUpdated;

			public ISelection Selection
			{
				get
				{
					//we need the actual variable to be able to remain null, so we know when to update it automatically
					return _selection ?? new Selection();
				}
				set
				{
					value = value ?? new Selection();

					if (!Object.Equals(value, _selection))
					{
						_selection = value;
						OnSelectionChanged();
					}
				}
			}

			public event EventHandler SelectionChanged;

			#endregion

			#region Private Methods

			private void OnInternalTreeUpdated(object sender, EventArgs e)
			{
				UpdateInternalTreeRoot();
				OnTreeUpdated();
			}

			private void OnSelectionChanged()
			{
				EventsHelper.Fire(SelectionChanged, this, EventArgs.Empty);
			}

			private void OnTreeChanged()
			{
				EventsHelper.Fire(TreeChanged, this, EventArgs.Empty);
			}

			private void OnTreeUpdated()
			{
				EventsHelper.Fire(TreeUpdated, this, EventArgs.Empty);
			}

			private void UpdateInternalTreeRoot()
			{
				ImageSetTreeGroupItem treeRoot = _internalTree;

				while (treeRoot.GetItems().Count == 0)
				{
					ReadOnlyCollection<ImageSetTreeGroupItem> childGroupItems = treeRoot.GetGroupItems();
					int nonEmptyChildGroupItems = 0;
					foreach (ImageSetTreeGroupItem childGroupItem in childGroupItems)
					{
						if (childGroupItem.GetAllItems().Count > 0)
							++nonEmptyChildGroupItems;
					}

					if (nonEmptyChildGroupItems == 1)
						treeRoot = childGroupItems[0];
					else
						break;
				}

				bool treeChanged = (_internalTreeRoot != treeRoot);

				_internalTreeRoot = treeRoot;
				UpdateSelection(false);
				ExpandToSelection();

				if (treeChanged)
					OnTreeChanged();
			}

			private void UpdateSelection(bool reset)
			{
				if (reset)
					_selection = null;

				if (_selection == null)
				{
					if (!String.IsNullOrEmpty(_primaryStudyInstanceUid))
					{
						foreach (ImageSetTreeItem item in _internalTreeRoot.GetAllItems())
						{
							if (item.ImageSet.Uid == _primaryStudyInstanceUid)
							{
								this.Selection = new Selection(item);
								break;
							}
						}
					}
				}
				else if (_selection.Item != null)
				{
					if (!IsInInternalTreeRoot((IImageSetTreeItem)_selection.Item))
						this.Selection = null;
				}
			}

			private bool IsInInternalTreeRoot(IImageSetTreeItem treeItem)
			{
				while (treeItem != null)
				{
					if (treeItem == _internalTreeRoot)
						return true;

					treeItem = treeItem.Parent;
				}

				return false;
			}

			private void ExpandToSelection()
			{
				if (_selection != null && _selection.Item != null)
					ExpandTo(_selection.Item as IImageSetTreeItem);
			}

			private void ExpandTo(IImageSetTreeItem item)
			{
				ImageSetTreeGroupItem parent = item.Parent;

				while (parent != _internalTreeRoot)
				{
					parent.IsExpanded = true;
					parent = parent.Parent;
				}

				_internalTreeRoot.IsExpanded = true;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				_imageSetGroups.Dispose();

				_internalTree.Updated -= OnInternalTreeUpdated;
				_internalTree.Dispose();
			}

			#endregion
		}

		#endregion

		#region IImageSetTreeItem interface

		private interface IImageSetTreeItem
		{
			ImageSetTreeGroupItem Parent { get; }

			string Description { get; }
		}

		#endregion

		#region Tree Binding

		private class ImageSetTreeItemBinding : TreeItemBindingBase
		{
			public override string GetNodeText(object item)
			{
				return ((IImageSetTreeItem) item).Description;
			}

			public override string GetTooltipText(object item)
			{
				return GetNodeText(item);
			}

			public override bool GetExpanded(object item)
			{
				if (item is ImageSetTreeGroupItem)
					return ((ImageSetTreeGroupItem) item).IsExpanded;

				return false;
			}

			public override void SetExpanded(object item, bool expanded)
			{
				if (item is ImageSetTreeGroupItem)
					((ImageSetTreeGroupItem)item).IsExpanded = expanded;
			}

			public override bool CanHaveSubTree(object item)
			{
				return item is ImageSetTreeGroupItem;
			}

			public override ITree GetSubTree(object item)
			{
				if (item is ImageSetTreeGroupItem)
					return ((ImageSetTreeGroupItem) item).Tree;

				return null;
			}
		}

		#endregion

		#region ImageSet Tree Group Item

		private class ImageSetTreeGroupItem : IImageSetTreeItem, IDisposable
		{
			#region Private Fields

			private readonly ImageSetTreeGroupItem _parent;
			private readonly FilteredGroup<IImageSet> _group;
			private readonly Tree<IImageSetTreeItem> _tree;
			private bool _isExpanded;

			private readonly IComparer<IImageSet> _imageSetComparer;

			#endregion

			private ImageSetTreeGroupItem(FilteredGroup<IImageSet> group, ImageSetTreeGroupItem parent)
				: this(group, parent._imageSetComparer)
			{
				_parent = parent;
			}

			public ImageSetTreeGroupItem(FilteredGroup<IImageSet> group, IComparer<IImageSet> imageSetComparer)
			{
				_group = group;
				_imageSetComparer = imageSetComparer;
				_tree = new Tree<IImageSetTreeItem>(new ImageSetTreeItemBinding());
				
				_group.ItemAdded += OnItemAdded;
				_group.ItemRemoved += OnItemRemoved;

				_group.ChildGroups.ItemAdded += OnChildGroupAdded;
				_group.ChildGroups.ItemRemoved += OnChildGroupRemoved;
				_group.ChildGroups.ItemChanging += OnChildGroupChanging;
				_group.ChildGroups.ItemChanged += OnChildGroupChanged;

				Initialize();
				_isExpanded = false;
			}

			#region Public Properties/Events

			public bool IsExpanded
			{
				get { return _isExpanded; }
				set { _isExpanded = value; }
			}

			public Tree<IImageSetTreeItem> Tree
			{
				get { return _tree; }
			}

			public event EventHandler Updated;

			#region IImageSetTreeItem Members

			public ImageSetTreeGroupItem Parent
			{
				get { return _parent; }
			}

			public string Description
			{
				get { return _group.Label; }
			}

			#endregion
			#endregion

			private void OnUpdated()
			{
				EventsHelper.Fire(Updated, this, EventArgs.Empty);
				if (Parent != null)
					Parent.OnUpdated();
			}

			private void Initialize()
			{
				List<IImageSet> imageSets = new List<IImageSet>(_group.Items);
				if (_imageSetComparer != null)
					imageSets.Sort(_imageSetComparer);
				
				foreach (IImageSet imageSet in imageSets)
					_tree.Items.Add(new ImageSetTreeItem(imageSet, this));

				foreach (FilteredGroup<IImageSet> childGroup in _group.ChildGroups)
					_tree.Items.Add(new ImageSetTreeGroupItem(childGroup, this));
			}

			#region Private Methods

			#region Group Handlers

			private void OnChildGroupAdded(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
			{
				ImageSetTreeGroupItem newGroupItem = new ImageSetTreeGroupItem(e.Item, this);
				_tree.Items.Add(newGroupItem);
				OnUpdated();
			}

			private void OnChildGroupRemoved(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
			{
				ImageSetTreeGroupItem groupItem = FindGroupItem(e.Item);
				if (groupItem != null)
				{
					_tree.Items.Remove(groupItem);
					groupItem.Dispose();
					OnUpdated();
				}
			}

			private ImageSetTreeGroupItem _changingGroupItem;

			private void OnChildGroupChanging(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
			{
				_changingGroupItem = FindGroupItem(e.Item);
			}

			private void OnChildGroupChanged(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
			{
				if (_changingGroupItem != null)
				{
					int replaceIndex = _tree.Items.IndexOf(_changingGroupItem);
					if (replaceIndex >= 0)
					{
						_tree.Items[replaceIndex] = new ImageSetTreeGroupItem(e.Item, this);
						OnUpdated();
					}

					_changingGroupItem.Dispose();
					_changingGroupItem = null;
				}
			}

			#endregion

			#region ImageSet Handlers

			private void OnItemAdded(object sender, ItemEventArgs<IImageSet> e)
			{
				ImageSetTreeItem newItem = new ImageSetTreeItem(e.Item, this);
				
				ReadOnlyCollection<ImageSetTreeItem> items = GetItems();
				List<IImageSet> imageSets = CollectionUtils.Map<ImageSetTreeItem, IImageSet>(
					items, delegate(ImageSetTreeItem item) { return item.ImageSet; });

				imageSets.Add(e.Item);

				if (_imageSetComparer != null)
					imageSets.Sort(_imageSetComparer);

				int insertIndex = imageSets.IndexOf(e.Item);
				if (insertIndex >= 0)
				{
					_tree.Items.Insert(insertIndex, newItem);
					OnUpdated();
				}
			}

			private void OnItemRemoved(object sender, ItemEventArgs<IImageSet> e)
			{
				foreach (IImageSetTreeItem item in _tree.Items)
				{
					if (item is ImageSetTreeItem)
					{
						ImageSetTreeItem treeItem = item as ImageSetTreeItem;
						if (treeItem.ImageSet == e.Item)
						{
							_tree.Items.Remove(treeItem);
							OnUpdated();
							break;
						}
					}
				}
			}

			#endregion

			private ImageSetTreeGroupItem FindGroupItem(FilteredGroup<IImageSet> filteredGroup)
			{
				ImageSetTreeGroupItem groupItem = CollectionUtils.SelectFirst(_tree.Items,
							delegate(IImageSetTreeItem treeItem)
							{
								if (treeItem is ImageSetTreeGroupItem)
									return ((ImageSetTreeGroupItem)treeItem)._group == filteredGroup;
								else
									return false;
							}) as ImageSetTreeGroupItem;

				return groupItem;
			}

			#endregion

			#region Public Methods

			public ReadOnlyCollection<ImageSetTreeItem> GetAllItems()
			{
				List<ImageSetTreeItem> items = new List<ImageSetTreeItem>();
				items.AddRange(GetItems());

				foreach (ImageSetTreeGroupItem groupItem in GetGroupItems())
					items.AddRange(groupItem.GetAllItems());

				return items.AsReadOnly();
			}

			public ReadOnlyCollection<ImageSetTreeItem> GetItems()
			{
				List<ImageSetTreeItem> items = new List<ImageSetTreeItem>();

				foreach (IImageSetTreeItem item in Tree.Items)
				{
					if (item is ImageSetTreeItem)
						items.Add(item as ImageSetTreeItem);
				}

				return items.AsReadOnly();
			}

			public ReadOnlyCollection<ImageSetTreeGroupItem> GetGroupItems()
			{
				List<ImageSetTreeGroupItem> groups = new List<ImageSetTreeGroupItem>();

				foreach (IImageSetTreeItem item in Tree.Items)
				{
					if (item is ImageSetTreeGroupItem)
						groups.Add(item as ImageSetTreeGroupItem);
				}

				return groups.AsReadOnly();
			}

			public override string ToString()
			{
				return this.Description;
			}

			#region IDisposable Members

			public void Dispose()
			{
				_group.ItemAdded -= OnItemAdded;
				_group.ItemRemoved -= OnItemRemoved;

				_group.ChildGroups.ItemAdded -= OnChildGroupAdded;
				_group.ChildGroups.ItemRemoved -= OnChildGroupRemoved;
				_group.ChildGroups.ItemChanging -= OnChildGroupChanging;
				_group.ChildGroups.ItemChanged -= OnChildGroupChanged;
			}

			#endregion
			#endregion
		}

		#endregion

		#region ImageSet Tree Item

		private class ImageSetTreeItem : IImageSetTreeItem
		{
			private readonly IImageSet _imageSet;
			private readonly ImageSetTreeGroupItem _parent;

			public ImageSetTreeItem(IImageSet imageSet, ImageSetTreeGroupItem parent)
			{
				_imageSet = imageSet;
				_parent = parent;
			}

			#region Public Properties

			public IImageSet ImageSet
			{
				get { return _imageSet; }
			}

			#region IImageSetTreeItem Members

			public ImageSetTreeGroupItem Parent
			{
				get { return _parent; }
			}

			public string Description
			{
				get { return _imageSet.Name; }
			}

			#endregion
			#endregion

			public override string ToString()
			{
				return this.Description;
			}
		}

		#endregion
	}
}
