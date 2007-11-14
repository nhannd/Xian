#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Trees
{
    public delegate string TextProviderDelegate<T>(T item);
    public delegate bool IsCheckedGetterDelegate<T>(T item);
    public delegate void IsCheckedSetterDelegate<T>(T item, bool value);
    public delegate IconSet IconSetProviderDelegate<T>(T item);
    public delegate IResourceResolver ResourceResolverProviderDelegate<T>(T item);
    public delegate bool CanHaveSubTreeDelegate<T>(T item);
    public delegate ITree TreeProviderDelegate<T>(T item);
    public delegate bool ShouldExpandSubTreeDelegate<T>(T item);
    public delegate DragDropKind CanAcceptDropDelegate<T>(T item, object dropData, DragDropKind kind);
    public delegate DragDropKind AcceptDropDelegate<T>(T item, object dropData, DragDropKind kind);

    /// <summary>
    /// A useful generic implementation of <see cref="ITreeItemBinding"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class TreeItemBinding<TItem> : TreeItemBindingBase
    {
        private TextProviderDelegate<TItem> _nodeTextProvider;
        private IsCheckedGetterDelegate<TItem> _isCheckedGetter;
        private IsCheckedSetterDelegate<TItem> _isCheckedSetter;
        private TextProviderDelegate<TItem> _tooltipTextProvider;
        private IconSetProviderDelegate<TItem> _iconSetIndexProvider;
        private ResourceResolverProviderDelegate<TItem> _resourceResolverProvider;
        private CanHaveSubTreeDelegate<TItem> _canHaveSubTreeHandler;
        private ShouldExpandSubTreeDelegate<TItem> _shouldInitiallyExpandSubTreeHandler;
        private TreeProviderDelegate<TItem> _subTreeProvider;
        private CanAcceptDropDelegate<TItem> _canAcceptDropHandler;
        private AcceptDropDelegate<TItem> _acceptDropHandler;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeTextProvider"></param>
        /// <param name="subTreeProvider"></param>
        public TreeItemBinding(TextProviderDelegate<TItem> nodeTextProvider, TreeProviderDelegate<TItem> subTreeProvider)
        {
            _nodeTextProvider = nodeTextProvider;
            _subTreeProvider = subTreeProvider;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeTextProvider"></param>
        public TreeItemBinding(TextProviderDelegate<TItem> nodeTextProvider)
            : this(nodeTextProvider, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TreeItemBinding()
            :this(null, null)
        {
        }

        /// <summary>
        /// Gets or sets the node text provider for this binding
        /// </summary>
        public TextProviderDelegate<TItem> NodeTextProvider
        {
            get { return _nodeTextProvider; }
            set { _nodeTextProvider = value; }
        }

        /// <summary>
        /// Gets or sets the node checked status provider for this binding
        /// </summary>
        public IsCheckedGetterDelegate<TItem> IsCheckedGetter
        {
            get { return _isCheckedGetter; }
            set { _isCheckedGetter = value; }
        }

        public IsCheckedSetterDelegate<TItem> IsCheckedSetter
        {
            get { return _isCheckedSetter; }
            set { _isCheckedSetter = value; }
        }

        /// <summary>
        /// Gets or sets the tooltip text provider for this binding
        /// </summary>
        public TextProviderDelegate<TItem> TooltipTextProvider
        {
            get { return _tooltipTextProvider; }
            set { _tooltipTextProvider = value; }
        }

        /// <summary>
        /// Gets or sets the iconset provider for this binding
        /// </summary>
        public IconSetProviderDelegate<TItem> IconSetProvider
        {
            get { return _iconSetIndexProvider; }
            set { _iconSetIndexProvider = value; }
        }

        /// <summary>
        /// Gets or sets the resource resolver provider for this binding
        /// </summary>
        public ResourceResolverProviderDelegate<TItem> ResourceResolverProvider
        {
            get { return _resourceResolverProvider; }
            set { _resourceResolverProvider = value; }
        }

        public CanHaveSubTreeDelegate<TItem> CanHaveSubTreeHandler
        {
            get { return _canHaveSubTreeHandler; }
            set { _canHaveSubTreeHandler = value; }
        }

        /// <summary>
        /// Gets or sets the subtree expansion state provider for this binding
        /// </summary>
        public ShouldExpandSubTreeDelegate<TItem> ShouldInitiallyExpandSubTreeHandler
        {
            get { return _shouldInitiallyExpandSubTreeHandler; }
            set { _shouldInitiallyExpandSubTreeHandler = value; }
        }

        /// <summary>
        /// Gets or sets the subtree provider for this binding
        /// </summary>
        public TreeProviderDelegate<TItem> SubTreeProvider
        {
            get { return _subTreeProvider; }
            set { _subTreeProvider = value; }
        }

        public CanAcceptDropDelegate<TItem> CanAcceptDropHandler
        {
            get { return _canAcceptDropHandler; }
            set { _canAcceptDropHandler = value; }
        }

        public AcceptDropDelegate<TItem> AcceptDropHandler
        {
            get { return _acceptDropHandler; }
            set { _acceptDropHandler = value; }
        }
	
	

        public override string GetNodeText(object item)
        {
            return _nodeTextProvider((TItem)item);
        }

        public override bool GetIsChecked(object item)
        {
            return _isCheckedGetter == null ? base.GetIsChecked(((TItem) item)) : _isCheckedGetter((TItem) item);
        }

        public override void SetIsChecked(object item, bool value)
        {
            if(_isCheckedSetter != null)
            {
                _isCheckedSetter((TItem) item, value);
            }
        }

        public override bool CanHaveSubTree(object item)
        {
            return _canHaveSubTreeHandler == null ? base.CanHaveSubTree(item) : _canHaveSubTreeHandler((TItem)item);
        }

        public override ITree GetSubTree(object item)
        {
            return _subTreeProvider == null ? base.GetSubTree(item) : _subTreeProvider((TItem)item);
        }

        public override bool ShouldInitiallyExpandSubTree(object item)
        {
             return _shouldInitiallyExpandSubTreeHandler == null ? base.ShouldInitiallyExpandSubTree(item) : _shouldInitiallyExpandSubTreeHandler((TItem)item);
        }

        public override string GetTooltipText(object item)
        {
            return _tooltipTextProvider == null ? base.GetTooltipText(item) : _tooltipTextProvider((TItem)item);
        }

        public override IconSet GetIconSet(object item)
        {
            return _iconSetIndexProvider == null ? base.GetIconSet(item) : _iconSetIndexProvider((TItem)item);
        }

        public override IResourceResolver GetResourceResolver(object item)
        {
            return _resourceResolverProvider == null ? base.GetResourceResolver(item) : _resourceResolverProvider((TItem)item);
        }

        public override DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return _canAcceptDropHandler == null ? base.CanAcceptDrop(item, dropData, kind) : _canAcceptDropHandler((TItem)item, dropData, kind);
        }

        public override DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return _acceptDropHandler == null ? base.AcceptDrop(item, dropData, kind) : _acceptDropHandler((TItem)item, dropData, kind);
        }

    }
}
