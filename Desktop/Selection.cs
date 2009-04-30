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
	// Note: It is important that this class retain immutable semantics.  Do not add mutator methods/properties to this class.
	
	/// <summary>
    /// Default implementation of <see cref="ISelection"/>.  
    /// </summary>
	public class Selection : ISelection, IEquatable<ISelection>
    {
		/// <summary>
		/// Represents an empty <see cref="ISelection"/>.
		/// </summary>
        public static readonly Selection Empty = new Selection();

        private List<object> _items = new List<object>();

		/// <summary>
		/// Constructor.
		/// </summary>
        public Selection()
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The single item that is selected.</param>
        public Selection(object item)
        {
            // if item == null, then don't add it, which gives us the Empty Selection
            if (item != null)
            {
                _items.Add(item);
            }
        }

		/// <summary>
		/// Constructor for multi-selection.
		/// </summary>
		/// <param name="items">The selected items.</param>
        public Selection(IEnumerable items)
        {
            foreach (object item in items)
                _items.Add(item);
        }

        #region ISelection Members

    	/// <summary>
    	/// Returns the set of items that are currently selected.
    	/// </summary>
    	public object[] Items
        {
            get { return _items.ToArray(); }
        }

		/// <summary>
		/// Returns the first item in the list of selected items, or null if nothing is selected.
		/// </summary>
        public object Item
        {
            get { return _items.Count > 0 ? _items[0] : null; }
        }

    	/// <summary>
    	/// Computes the union of this selection with another and returns it.
    	/// </summary>
    	public ISelection Union(ISelection other)
        {
            List<object> sum = new List<object>();

            // add all the items from the other selection
            sum.AddRange(other.Items);

            // add only the items from this selection not contained in the other selection
            sum.AddRange(_items.FindAll(delegate(object x) { return !other.Contains(x); }));

            return new Selection(sum);
        }

    	/// <summary>
    	/// Computes the intersection of this selection with another and returns it.
    	/// </summary>
    	public ISelection Intersect(ISelection other)
        {
            // return every item in this selection also contained in the other selection
            return new Selection(_items.FindAll(delegate(object x) { return other.Contains(x); }));
        }

    	/// <summary>
    	/// Returns an <see cref="ISelection"/> that contains every item contained
    	/// in this one that doesn't exist in <param name="other" />.
    	/// </summary>
    	public ISelection Subtract(ISelection other)
        {
            // return every item in this selection not contained in the other selection
            return new Selection(_items.FindAll(delegate(object x) { return !other.Contains(x); }));
        }

    	/// <summary>
    	/// Determines whether this selection contains the input object.
    	/// </summary>
    	public bool Contains(object item)
        {
            return _items.Contains(item);
        }

        #endregion

        #region IEquatable<ISelection> Members

		/// <summary>
		/// Gets whether or not this <see cref="ISelection"/> is considered equivalent to <paramref name="other"/>.
		/// </summary>
        public bool Equals(ISelection other)
        {
            if (other == null)
                return false;

            // false if not same number of elements
            if (other.Items.Length != _items.Count)
                return false;

            // because we now know that they contain the same number of elements,
            // they are equal if every item in this selection is contained in the other selection
            return _items.TrueForAll(
                delegate(object x) { return other.Contains(x); });
        }

        #endregion

		/// <summary>
		/// Gets whether or not this object is considered equal to another.
		/// </summary>
        public override bool Equals(object obj)
        {
            ISelection that = obj as ISelection;
            return this.Equals(that);
        }

		/// <summary>
		/// Gets a hash code for this object.
		/// </summary>
        public override int GetHashCode()
        {
            int n = 0;
            foreach (object item in _items)
            {
                n ^= item.GetHashCode();
            }
            return n;
        }
    }
}
