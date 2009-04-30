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
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Enumerates the types of item changes.
    /// </summary>
    public enum ItemChangeType
    {
        /// <summary>
        /// An item was added to the table.
        /// </summary>
        ItemAdded,

		/// <summary>
		/// An item was inserted to the table.
		/// </summary>
		ItemInserted,
		
        /// <summary>
        /// An item in the table was changed.
        /// </summary>
        ItemChanged,

        /// <summary>
        /// An item was removed from the table.
        /// </summary>
        ItemRemoved,

        /// <summary>
        /// All items in the table may have changed.
        /// </summary>
        Reset
    }

    /// <summary>
    /// Provides data for the <see cref="IItemCollection.ItemsChanged"/> event.
    /// </summary>
    public class ItemChangedEventArgs : EventArgs
    {
        private readonly object _item;
        private readonly int _itemIndex;
        private readonly ItemChangeType _changeType;

        internal ItemChangedEventArgs(ItemChangeType changeType, int itemIndex, object item)
        {
            _changeType = changeType;
            _itemIndex = itemIndex;
            _item = item;
        }

        /// <summary>
        /// Gets the type of change that occured.
        /// </summary>
        public ItemChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// Gets the index of the item that changed.
        /// </summary>
        public int ItemIndex
        {
            get { return _itemIndex; }
        }

        /// <summary>
        /// Gets the item that has changed.
        /// </summary>
        public object Item
        {
            get { return _item; }
        }
    }

	// TODO: get rid of these interfaces.
	
	/// <summary>
    /// Defines the interface to the collection of items.
    /// </summary>
    /// <remarks>
    /// Do not implement this interface.  It is likely to be removed
    /// in subsequent versions of the framework and is 
    /// not considered part of the public API.
    /// </remarks>
    public interface IItemCollection : IList
    {
        /// <summary>
        /// Occurs when an item in the collection has changed.
        /// </summary>
        event EventHandler<ItemChangedEventArgs> ItemsChanged;
    }

    /// <summary>
    /// Defines the interface to the collection of items.
    /// </summary>
	/// <remarks>
	/// Do not implement this interface.  It is likely to be removed
	/// in subsequent versions of the framework and is 
	/// not considered part of the public API.
	/// </remarks>
	/// <typeparam name="TItem">The item type.</typeparam>
    public interface IItemCollection<TItem> : IItemCollection, IList<TItem>
    {
    }
}
