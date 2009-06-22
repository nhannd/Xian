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
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IImageBox"/> objects.
	/// </summary>
	public class ImageBoxCollection : ObservableList<IImageBox>
	{
		private bool _locked;

		/// <summary>
		/// Initializes a new instance of <see cref="ImageBoxCollection"/>.
		/// </summary>
		internal ImageBoxCollection()
		{
		}

		/// <summary>
		/// Creates a copy of the object.
		/// </summary>
		/// <param name="collection"></param>
		/// <remarks>
		/// Creates a <i>shallow</i> copy.  That is, only references to objects
		/// in the collection are copied.
		/// </remarks>
		internal ImageBoxCollection(IEnumerable<IImageBox> collection)
			: base(collection)
		{
		}

		//NOTE: opted not to override base.IsReadOnly b/c the semantic is slightly different.
		//This is not a 'read only' list, rather the parent object has temporarily locked it.
		internal bool Locked
		{
			get { return _locked; }
			set { _locked = value; }
		}

		/// <summary>
		/// Adds the specified item to the list.
		/// </summary>
		public override void Add(IImageBox item)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");
				
			base.Add(item);
		}

		/// <summary>
		/// Inserts <paramref name="item"/> at the specified <paramref name="index"/>.
		/// </summary>
		public override void Insert(int index, IImageBox item)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");

			base.Insert(index, item);
		}

		/// <summary>
		/// Removes the specified <paramref name="item"/> from the list.
		/// </summary>
		/// <returns>True if the item was in the list and was removed.</returns>
		public override bool Remove(IImageBox item)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");
				
			return base.Remove(item);
		}

		/// <summary>
		/// Removes the item at the specified <paramref name="index"/>.
		/// </summary>
		public override void RemoveAt(int index)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");

			base.RemoveAt(index);
		}

		/// <summary>
		/// Clears the list.
		/// </summary>
		public override void Clear()
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");

			base.Clear();
		}

		/// <summary>
		/// Gets or sets the item at the specified index.
		/// </summary>
		public override IImageBox this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				if (_locked)
					throw new InvalidOperationException("The image box collection is locked.");

				base[index] = value;
			}
		}
	}
}
