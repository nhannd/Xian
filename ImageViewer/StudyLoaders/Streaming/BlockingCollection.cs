#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	//Similar to the BlockingQueue, except that items can be added and removed.
	//TODO: the behaviour of this class and it's API should be refactored if it's going to be reused.
	public class BlockingCollection<T> : ICollection<T>, IBlockingEnumerator<T>
	{
		private readonly object _syncLock = new object();
		private readonly List<T> _items = new List<T>();
		private volatile bool _continueBlocking = true;

		#region IBlockingCollection<T> Members

		public bool ContinueBlocking
		{
			get { return _continueBlocking; }
			set
			{
				if (_continueBlocking == value)
					return;

				_continueBlocking = value;
				if (!_continueBlocking)
				{
					lock (_syncLock)
					{
						Monitor.PulseAll(_syncLock);
					}
				}
			}
		}

		#endregion

		private IEnumerable<T> GetItems()
		{
			while (_continueBlocking)
			{
				T item;

				lock (_syncLock)
				{
					if (_items.Count == 0)
					{
						Monitor.Wait(_syncLock);
						if (_items.Count == 0)
							continue;
					}

					item = _items[0];
					_items.RemoveAt(0);
				}

				yield return item;
			}
		}

		public void Clear(out List<T> itemsRemoved)
		{
			lock (_syncLock)
			{
				itemsRemoved = new List<T>(_items);
				_items.Clear();
			}
		}

		#region ICollection<T> Members

		public void Add(T item)
		{
			lock (_syncLock)
			{
				if (!_items.Contains(item))
				{
					_items.Add(item);
					Monitor.Pulse(_syncLock);
				}
			}
		}

		public void AddRange(IEnumerable<T> items)
		{
			lock (_syncLock)
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				foreach (T item in items)
				{
					if (!_items.Contains(item))
					{
						_items.Add(item);
						Monitor.Pulse(_syncLock);
					}
				}

				clock.Stop();
				PerformanceReportBroker.PublishReport("BlockingCollection", "AddRange", clock.Seconds);
			}
		}

		public bool Remove(T item)
		{
			lock (_syncLock)
			{
				return _items.Remove(item);
			}
		}

		public void Clear()
		{
			lock (_syncLock)
			{
				_items.Clear();
			}
		}

		public bool Contains(T item)
		{
			lock (_syncLock)
			{
				return _items.Contains(item);
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			lock (_syncLock)
			{
				_items.CopyTo(array, arrayIndex);
			}
		}

		public int Count
		{
			get
			{
				lock (_syncLock)
				{
					return _items.Count;
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return GetItems().GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetItems().GetEnumerator();
		}

		#endregion
	}
}