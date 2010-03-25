#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public abstract class AbstractActionModelTreeNode
	{
		private event EventHandler _parentChanged;

		private AbstractActionModelTreeBranch _parent = null;
		private bool _isEditable = false;
		private bool _hidden = false;
		private string _resourceKey = string.Empty;
		private string _label = string.Empty;
		private string _tooltip = string.Empty;

		protected AbstractActionModelTreeNode(string resourceKey, string label)
		{
			_resourceKey = resourceKey;
			_label = label;
		}

		protected AbstractActionModelTreeNode(PathSegment pathSegment)
		{
			Platform.CheckForNullReference(pathSegment, "pathSegment");
			_resourceKey = pathSegment.ResourceKey;
			_label = pathSegment.LocalizedText;
		}

		public bool IsEditable
		{
			get { return _isEditable; }
			set { _isEditable = value; }
		}

		public virtual bool Hidden
		{
			get { return _hidden; }
			protected set { _hidden = value; }
		}

		public string ResourceKey
		{
			get { return _resourceKey; }
			set { _resourceKey = value; }
		}

		public string Label
		{
			get { return _label; }
			set { _label = value; }
		}

		public string CanonicalLabel
		{
			get
			{
				if (string.IsNullOrEmpty(_label))
					return string.Empty;
				return string.Join("", _label.Split(new char[] {'&'}, 2));
			}
		}

		public string Tooltip
		{
			get { return _tooltip; }
			set { _tooltip = value; }
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
			remove {_parentChanged -= value; } 
		}

		protected virtual void OnParentChanged()
		{
			EventsHelper.Fire(_parentChanged, this, EventArgs.Empty);
		}

		public bool IsDescendantOf(AbstractActionModelTreeBranch node)
		{
			if (ReferenceEquals(node, null) || ReferenceEquals(_parent, null))
				return false;
			if (ReferenceEquals(_parent, node))
				return true;
			return _parent.IsDescendantOf(node);
		}

		public virtual DragDropKind CanAcceptDrop(object dropData, DragDropKind dragDropKind)
		{
			return DragDropKind.None;
		}

		public virtual DragDropKind AcceptDrop(object dropData, DragDropKind dragDropKind)
		{
			return DragDropKind.None;
		}
	}
}