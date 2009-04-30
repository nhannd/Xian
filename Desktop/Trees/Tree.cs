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

using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// A useful generic implementation of <see cref="ITree"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class Tree<TItem> : ITree
    {
        private ITreeItemBinding _binding;
        private ItemCollection<TItem> _items;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="binding">The tree item binding.</param>
        public Tree(ITreeItemBinding binding)
            :this(binding, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binding">The tree item binding.</param>
        /// <param name="items">The set of items that are initially contained in this tree.</param>
        public Tree(ITreeItemBinding binding, IEnumerable<TItem> items)
        {
            _binding = binding;
            _items = new ItemCollection<TItem>();
            if (items != null)
            {
                _items.AddRange(items);
            }
        }

        /// <summary>
		/// Gets the <see cref="IItemCollection{TItem}"/> associated with this tree.
        /// </summary>
        public ItemCollection<TItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the item binding associated with this tree.
        /// </summary>
        public ITreeItemBinding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        #region ITree Members

		/// <summary>
		/// Gets the <see cref="IItemCollection"/> associated with this tree.
		/// </summary>
		IItemCollection ITree.Items
        {
            get { return _items; }
        }

        #endregion
    }
}
