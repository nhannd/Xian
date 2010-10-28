#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	// TODO CR (Apr 10): Rename this to ActionModelConfigurationNode, and all subclasses to similar names
	public abstract class AbstractActionModelTreeNode
	{
		private static readonly Regex _escapeRegex = new Regex(@"&(.)", RegexOptions.Compiled);

		private event EventHandler _parentChanged;

		private AbstractActionModelTreeBranch _parent = null;
		private IconSet _iconSet = null;
		private IResourceResolver _resourceResolver = null;
		private PathSegment _pathSegment = null;
		private string _canonicalLabel = null;
		private string _tooltip = string.Empty;
		private CheckState _checkState = CheckState.Unchecked;
		private bool _isExpanded = false;
		private bool _isHighlighted = false;

		protected AbstractActionModelTreeNode(PathSegment pathSegment)
		{
			Platform.CheckForNullReference(pathSegment, "pathSegment");
			_pathSegment = pathSegment;
		}

		internal PathSegment PathSegment
		{
			get { return _pathSegment; }
		}

		public string Label
		{
			get { return _pathSegment.LocalizedText; }
			set
			{
				if (!this.RequestValidation("Label", value))
					return;

				if (_pathSegment.LocalizedText != value)
				{
					_pathSegment = new PathSegment(value, value);
					_canonicalLabel = null;
					this.NotifyValidated("Label", value);
					this.OnLabelChanged();
				}
			}
		}

		public string CanonicalLabel
		{
			get
			{
				if (_canonicalLabel == null)
				{
					_canonicalLabel = _escapeRegex.Replace(this.Label, "$1");
				}
				return _canonicalLabel;
			}
		}

		public string Tooltip
		{
			get { return _tooltip; }
			set
			{
				if (!this.RequestValidation("Tooltip", value))
					return;

				if (_tooltip != value)
				{
					_tooltip = value;
					this.NotifyValidated("Tooltip", value);
					this.OnTooltipChanged();
				}
			}
		}

		public CheckState CheckState
		{
			get { return _checkState; }
			set
			{
				if (!this.RequestValidation("CheckState", value))
					return;

				if (_checkState != value)
				{
					_checkState = value;
					this.NotifyValidated("CheckState", value);
					this.OnCheckStateChanged();
				}
			}
		}

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (!this.RequestValidation("IsExpanded", value))
					return;

				if (_isExpanded != value)
				{
					_isExpanded = value;
					this.NotifyValidated("IsExpanded", value);
					this.OnIsExpandedChanged();
				}
			}
		}

		public bool IsHighlighted
		{
			get { return _isHighlighted; }
			set
			{
				if (!this.RequestValidation("IsHighlighted", value))
					return;

				if (_isHighlighted != value)
				{
					_isHighlighted = value;
					this.NotifyValidated("IsHighlighted", value);
					this.OnIsHighlightedChanged();
				}
			}
		}

		public IconSet IconSet
		{
			get { return _iconSet; }
			protected set
			{
				if (_iconSet != value)
				{
					_iconSet = value;
					this.NotifyItemChanged();
				}
			}
		}

		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			protected set
			{
				if (_resourceResolver != value)
				{
					_resourceResolver = value;
					this.NotifyItemChanged();
				}
			}
		}

		public AbstractActionModelTreeBranch Parent
		{
			get { return _parent; }
			internal set
			{
				if (!ReferenceEquals(_parent, value))
				{
					_parent = value;
					this.OnParentChanged();
				}
			}
		}

		public event EventHandler ParentChanged
		{
			add { _parentChanged += value; }
			remove { _parentChanged -= value; }
		}

		protected virtual void OnParentChanged()
		{
			EventsHelper.Fire(_parentChanged, this, EventArgs.Empty);
		}

		protected virtual void NotifyItemChanged()
		{
			if (!ReferenceEquals(_parent, null))
			{
				_parent.NotifyChildChanged(this);
			}
		}

		protected virtual void OnCheckStateChanged()
		{
			this.NotifyItemChanged();
		}

		protected virtual void OnIsExpandedChanged()
		{
			this.NotifyItemChanged();
		}

		protected virtual void OnIsHighlightedChanged()
		{
			this.NotifyItemChanged();
		}

		protected virtual void OnLabelChanged()
		{
			this.NotifyItemChanged();
		}

		protected virtual void OnTooltipChanged()
		{
			this.NotifyItemChanged();
		}

		protected internal bool RequestValidation(string propertyName, object value)
		{
			if (!ReferenceEquals(this.Parent, null))
			{
				return this.Parent.RequestValidation(this, propertyName, value);
			}
			return true;
		}

		protected internal void NotifyValidated(string propertyName, object value)
		{
			if (!ReferenceEquals(this.Parent, null))
			{
				this.Parent.NotifyValidated(this, propertyName, value);
			}
		}

		public bool IsDescendantOf(AbstractActionModelTreeBranch node)
		{
			if (ReferenceEquals(node, null) || ReferenceEquals(_parent, null))
				return false;
			if (ReferenceEquals(_parent, node))
				return true;
			return _parent.IsDescendantOf(node);
		}

		public virtual DragDropKind CanAcceptDrop(object dropData, DragDropKind dragDropKind, DragDropPosition dragDropPosition)
		{
			// drop target must have a parent, otherwise there is no concept of "sibling"
			if (dragDropPosition != DragDropPosition.Default && !ReferenceEquals(this.Parent, null))
			{
				if (dropData is AbstractActionModelTreeNode)
				{
					AbstractActionModelTreeNode droppedNode = (AbstractActionModelTreeNode) dropData;
					if (dragDropKind == DragDropKind.Move)
					{
						AbstractActionModelTreeNode sibling = null;
						if (droppedNode.Parent != null)
						{
							int index = droppedNode.Parent.Children.IndexOf(droppedNode) + (dragDropPosition == DragDropPosition.After ? -1 : 1);
							if (index >= 0 && index < droppedNode.Parent.Children.Count)
								sibling = droppedNode.Parent.Children[index];
						}

						// to drag-move, we can't be dragging immediately before/after ourself, or onto one of our descendants
						if (!ReferenceEquals(this, droppedNode)
						    && !ReferenceEquals(this, sibling)
						    && !this.IsDescendantOf(droppedNode as AbstractActionModelTreeBranch))
							return dragDropKind;
					}
				}
			}
			return DragDropKind.None;
		}

		public virtual DragDropKind AcceptDrop(object dropData, DragDropKind dragDropKind, DragDropPosition dragDropPosition)
		{
			// drop target must have a parent, otherwise there is no concept of "sibling"
			if (dragDropPosition != DragDropPosition.Default && !ReferenceEquals(this.Parent, null))
			{
				if (dropData is AbstractActionModelTreeNode)
				{
					AbstractActionModelTreeNode droppedNode = (AbstractActionModelTreeNode) dropData;
					if (dragDropKind == DragDropKind.Move)
					{
						AbstractActionModelTreeNode sibling = null;
						if (droppedNode.Parent != null)
						{
							int index = droppedNode.Parent.Children.IndexOf(droppedNode) + (dragDropPosition == DragDropPosition.After ? -1 : 1);
							if (index >= 0 && index < droppedNode.Parent.Children.Count)
								sibling = droppedNode.Parent.Children[index];
						}

						// to drag-move, we can't be dragging immediately before/after ourself, or onto one of our descendants
						if (!ReferenceEquals(this, droppedNode)
						    && !ReferenceEquals(this, sibling)
						    && !this.IsDescendantOf(droppedNode as AbstractActionModelTreeBranch))
						{
							if (droppedNode.Parent != null)
								droppedNode.Parent.Children.Remove(droppedNode);
							this.Parent.Children.Insert(this.Parent.Children.IndexOf(this) + (dragDropPosition == DragDropPosition.After ? 1 : 0), droppedNode);
							return dragDropKind;
						}
					}
				}
			}
			return DragDropKind.None;
		}
	}
}