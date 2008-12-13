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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Clipboard
{
	internal class ClipboardItemList : IList<IClipboardItem>
	{
		private readonly BindingList<IClipboardItem> _bindingList;

		public ClipboardItemList(BindingList<IClipboardItem> bindingList)
		{
			_bindingList = bindingList;
		}

		public BindingList<IClipboardItem> BindingList
		{
			get { return _bindingList; }	
		}

		#region IList<IClipboardItem> Members

		public int IndexOf(IClipboardItem item)
		{
			return _bindingList.IndexOf(item);
		}

		public void Insert(int index, IClipboardItem item)
		{
			_bindingList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			if (index < _bindingList.Count)
			{
				if (((ClipboardItem)_bindingList[index]).Locked)
					throw new InvalidOperationException("Unable to remove item because it is locked.");

				_bindingList.RemoveAt(index);
			}
		}

		public IClipboardItem this[int index]
		{
			get
			{
				return _bindingList[index];
			}
			set
			{
				throw new InvalidOperationException("Cannot set items via the indexer.");
			}
		}

		#endregion

		#region ICollection<IClipboardItem> Members

		public void Add(IClipboardItem item)
		{
			_bindingList.Add(item);
		}

		public void Clear()
		{
			foreach (IClipboardItem item in _bindingList)
			{
				if (((ClipboardItem)item).Locked)
					throw new InvalidOperationException("Unable to clear the list; there is a locked item.");
			}

			_bindingList.Clear();
		}

		public bool Contains(IClipboardItem item)
		{
			return _bindingList.Contains(item);
		}

		public void CopyTo(IClipboardItem[] array, int arrayIndex)
		{
			_bindingList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _bindingList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IClipboardItem item)
		{
			if (((ClipboardItem)item).Locked)
				throw new InvalidOperationException("Unable to remove item because it is locked.");

			return _bindingList.Remove(item);
		}

		#endregion

		#region IEnumerable<IClipboardItem> Members

		public IEnumerator<IClipboardItem> GetEnumerator()
		{
			return _bindingList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _bindingList.GetEnumerator();
		}

		#endregion
	}
}
