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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IDisplaySet"/> objects.
	/// </summary>
	public class DisplaySetCollection : ObservableList<IDisplaySet>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="DisplaySetCollection"/>.
		/// </summary>
		internal DisplaySetCollection()
		{
		}

		internal static IComparer<IDisplaySet> GetDefaultComparer()
		{
			return new DisplaySetNumberComparer();
		}

		/// <summary>
		/// The comparer that was last used to sort the collection, via <see cref="Sort"/>.
		/// </summary>
		/// <remarks>
		/// When an item is added to or replaced, this value is set to null.  When an item is
		/// simply removed, the sort order is maintained, so this value also will not change.
		/// </remarks>
		public IComparer<IDisplaySet> SortComparer { get; internal set; }

		/// <summary>
		/// Sorts the collection using <see cref="SortComparer"/>.
		/// </summary>
		/// <remarks>
		/// If <see cref="SortComparer"/> is null, it is first set to a default one.
		/// </remarks>
		public void Sort()
		{
			if (SortComparer == null)
				SortComparer = GetDefaultComparer();
			Sort(SortComparer);
		}

		/// <summary>
		/// Sorts the collection with the given comparer.
		/// </summary>
		public sealed override void Sort(IComparer<IDisplaySet> sortComparer)
		{
			Platform.CheckForNullReference(sortComparer, "comparer");
			SortComparer = sortComparer;
			base.Sort(SortComparer);
		}

		protected override void OnItemAdded(ListEventArgs<IDisplaySet> e)
		{
			SortComparer = null; //we don't know the sort order anymore.
			base.OnItemAdded(e);
		}

		protected override void OnItemChanged(ListEventArgs<IDisplaySet> e)
		{
			SortComparer = null;//we don't know the sort order anymore.
			base.OnItemChanged(e);
		}
	}
}