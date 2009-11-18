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
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
	internal class LargeObjectContainerCache : IEnumerable<ILargeObjectContainer>
	{
		#region Enumerator class

		private class Enumerator : IEnumerator<ILargeObjectContainer>
		{
			private readonly LargeObjectContainerCache _cache;
			private readonly IEnumerator<KeyValuePair<Guid, Item>> _realEnumerator;
			private ILargeObjectContainer _current;

			public Enumerator(LargeObjectContainerCache cache)
			{
				_cache = cache;
				_realEnumerator = _cache._largeObjectContainers.GetEnumerator();
			}

			#region IEnumerator<ILargeObjectContainer> Members

			public ILargeObjectContainer Current
			{
				get
				{
					if (_current == null)
						throw new InvalidOperationException("The enumerator is in an invalid state.");

					return _current;
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				_current = null;
				_realEnumerator.Dispose();
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get { return _current; }
			}

			public bool MoveNext()
			{
				bool result = false;
				bool keepMoving = true;

				while (keepMoving)
				{
					result = _realEnumerator.MoveNext();
					if (result)
					{
						_current = _realEnumerator.Current.Value.LargeObjectContainer;
						if (_current != null)
							keepMoving = false;
					}
					else
					{
						_current = null;
						keepMoving = false;
					}
				}

				return result;
			}

			public void Reset()
			{
				_cache.CleanupDeadItems(false);
				_current = null;
				_realEnumerator.Reset();
			}

			#endregion
		}

		#endregion

		#region Cache Item

		private class Item
		{
			private WeakReference _reference;

			public Item(ILargeObjectContainer container)
			{
				_reference = new WeakReference(container);
			}

			public ILargeObjectContainer LargeObjectContainer
			{
				get
				{
					try
					{
						ILargeObjectContainer container = null;
						if (_reference != null)
						{
							container = _reference.Target as ILargeObjectContainer;
							if (container == null)
								_reference = null;
						}

						return container;
					}
					catch (InvalidOperationException)
					{
						_reference = null;
						return null;
					}
				}
			}
		}

		#endregion

		#region Private Fields

		private readonly Dictionary<Guid, Item> _largeObjectContainers;
		private long _lastLargeObjectBytesCount;
		private volatile int _lastLargeObjectContainerCount;
		private volatile int _lastLargeObjectCount;

		#endregion

		public LargeObjectContainerCache()
		{
			_largeObjectContainers = new Dictionary<Guid, Item>();
		}

		#region Properties

		public long LastLargeObjectBytesCount
		{
			get { return _lastLargeObjectBytesCount; }	
		}

		public int LastLargeObjectContainerCount
		{
			get { return _lastLargeObjectContainerCount; }
		}

		public int LastLargeObjectCount
		{
			get { return _lastLargeObjectCount; }	
		}

		public bool IsEmpty
		{
			get { return _largeObjectContainers.Count == 0; }
		}

		#endregion

		#region Methods

		internal void CleanupDeadItems(bool updateEstimates)
		{
			List<Guid> keysToRemove = new List<Guid>();
			bool alreadyLogged = false;

			if (updateEstimates)
			{
				_lastLargeObjectBytesCount = 0;
				_lastLargeObjectCount = 0;
			}

			foreach (KeyValuePair<Guid, Item> item in _largeObjectContainers)
			{
				try
				{
					ILargeObjectContainer container = item.Value.LargeObjectContainer;
					if (container == null)
					{
						keysToRemove.Add(item.Key);
					}
					else if (updateEstimates)
					{
						_lastLargeObjectBytesCount += container.BytesHeldCount;
						_lastLargeObjectCount += container.LargeObjectCount;
					}
				}
				catch(Exception e)
				{
					if (!alreadyLogged)
					{
						alreadyLogged = true;
						Platform.Log(LogLevel.Warn, e, "An error occurred trying to clean up dead large object cache items.");
					}
				}
			}

			foreach (Guid keyToRemove in keysToRemove)
				_largeObjectContainers.Remove(keyToRemove);

			if (updateEstimates)
				_lastLargeObjectContainerCount = _largeObjectContainers.Count;
		}

		public void Add(ILargeObjectContainer container)
		{
			Guid identifier = container.Identifier;
			Item item;
			if (!_largeObjectContainers.TryGetValue(identifier, out item))
			{
				item = new Item(container);
				_largeObjectContainers.Add(identifier, item);
			}
			else
			{
				ILargeObjectContainer existing = item.LargeObjectContainer;
				if (existing == null || !ReferenceEquals(container, existing))
				{
					_largeObjectContainers[identifier] = new Item(container);
				}
			}
		}

		public bool Remove(ILargeObjectContainer item)
		{
			return _largeObjectContainers.Remove(item.Identifier);
		}

		#region IEnumerable<ILargeObjectContainer> Members

		public IEnumerator<ILargeObjectContainer> GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion
		#endregion
	}
}