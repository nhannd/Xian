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

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	//Note: not used right now, but it works so we'll keep it around.

	//Similar to the BlockingQueue, except that items can be added and removed.
	internal class BlockingCollection<T> : ICollection<T>, IBlockingEnumerator<T>
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
					lock(_syncLock)
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
			lock(_syncLock)
			{
				return _items.Remove(item);
			}
		}

		public void Clear()
		{
			lock(_syncLock)
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
				lock(_syncLock)
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