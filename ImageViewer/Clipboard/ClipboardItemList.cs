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
				if (_bindingList[index].Locked)
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
				throw new MemberAccessException("Cannot set items via the indexer.");
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
				if (item.Locked)
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
			if (item.Locked)
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
