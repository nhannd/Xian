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
using System.Diagnostics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal static class ComposedLutCache
	{
		#region Cached Lut
		
		public interface ICachedLut : IComposedLut, IDisposable
		{
		}

		public static ICachedLut GetLut(LutCollection sourceLuts)
		{
			Platform.CheckForNullReference(sourceLuts, "sourceLuts");
			return new CachedLutProxy(sourceLuts);
		}
		
		private class CachedLutProxy : ICachedLut
		{
			private readonly LutCollection _sourceLuts;
			private CacheItemProxy _cacheItemProxy;

			internal CachedLutProxy(LutCollection sourceLuts)
			{
				_sourceLuts = sourceLuts;
			}

			#region Cache Item Proxy

			private CacheItemProxy CacheItemProxy
			{
				get
				{
					_sourceLuts.SyncMinMaxValues();
					_sourceLuts.Validate();

					if (_cacheItemProxy == null)
					{
						_cacheItemProxy = CreateItemProxy(_sourceLuts);
					}
					else if (_cacheItemProxy.Key != GetKey(_sourceLuts))
					{
						//Trace.WriteLine("Detected cache item key != lut collection key", "LUT");
						_cacheItemProxy.Dispose();
						_cacheItemProxy = CreateItemProxy(_sourceLuts);
					}

					return _cacheItemProxy;
				}	
			}

			private void DisposeCacheItemProxy()
			{
				if (_cacheItemProxy != null)
				{
					_cacheItemProxy.Dispose();
					_cacheItemProxy = null;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					DisposeCacheItemProxy();
					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}

			#endregion
			#endregion

			#region IComposedLut Members

			private IComposedLut RealLut
			{
				get { return CacheItemProxy.GetLut(_sourceLuts); }
			}

			public int[] Data
			{
				get { return RealLut.Data; }
			}

			#endregion

			#region ILut Members

			public int MinInputValue
			{
				get { return RealLut.MinInputValue; }
			}

			public int MaxInputValue
			{
				get { return RealLut.MaxInputValue; }
			}

			public int MinOutputValue
			{
				get { return RealLut.MinOutputValue; }
			}

			public int MaxOutputValue
			{
				get { return RealLut.MaxOutputValue; }
			}

			public int this[int index]
			{
				get { return RealLut[index]; }
			}

			#endregion
		}

		#endregion
		#region Cache Item

		private interface ICacheItem : IDisposable
		{
			string Key { get; }
			IComposedLut GetLut(LutCollection sourceLuts);
		}

		#region Cache Item Proxy

		private class CacheItemProxy : ICacheItem
		{
			private ReferenceCountedObjectWrapper<CacheItem> _wrapper;
			private CacheItem _cacheItem;

			internal CacheItemProxy(ReferenceCountedObjectWrapper<CacheItem> wrapper)
			{
				_wrapper = wrapper;
				_cacheItem = _wrapper.Item;
				_wrapper.IncrementReferenceCount();
			}

			internal ReferenceCountedObjectWrapper<CacheItem> Wrapper
			{
				get { return _wrapper; }
			}

			#region IItem Members

			public string Key
			{
				get { return _cacheItem.Key; }
			}

			public IComposedLut GetLut(LutCollection sourceLuts)
			{
				return _cacheItem.GetLut(sourceLuts);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					if (_wrapper == null)
						return;

					_wrapper.DecrementReferenceCount();
					OnProxyDisposed(this);
					_wrapper = null;
					_cacheItem = null;

					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}

			#endregion
		}

		#endregion

		#region Cache Item

		private class CacheItem : ICacheItem, ILargeObjectContainer
		{
			private readonly LargeObjectContainerData _largeObjectData;
			private readonly string _key;
			private readonly BufferCache<int> _bufferCache = SharedBufferCache;

			private readonly object _syncLock = new object();
			private volatile ComposedLut _realComposedLut;

			internal CacheItem(string key)
			{
				_key = key;
				_largeObjectData = new LargeObjectContainerData(Guid.NewGuid()){RegenerationCost = RegenerationCost.Low};
			}

			~CacheItem()
			{
				_containsDeadItems = true;
			}

			public string Key
			{
				get { return _key; }
			}

			public IComposedLut GetLut(LutCollection sourceLuts)
			{
				IComposedLut lut = _realComposedLut;
				if (lut != null)
					return lut;

				lock (_syncLock)
				{
					if (_realComposedLut != null)
						return _realComposedLut;

					//Trace.WriteLine(String.Format("Creating Composed Lut '{0}'", Key), "LUT");

					_realComposedLut = new ComposedLut(sourceLuts, _bufferCache);
					//just use the creation time as the "last access time", otherwise it can get expensive when called in a tight loop.
					_largeObjectData.UpdateLastAccessTime();
					_largeObjectData.BytesHeldCount = _realComposedLut.Data.Length * sizeof(int);
					_largeObjectData.LargeObjectCount = 1;
					MemoryManager.Add(this);
					Diagnostics.OnLargeObjectAllocated(_largeObjectData.BytesHeldCount);

					return _realComposedLut;
				}
			}

			private void Unload(bool disposing)
			{
				if (_realComposedLut == null)
					return;

				lock (_syncLock)
				{
					if (_realComposedLut == null)
						return;

					Diagnostics.OnLargeObjectReleased(_largeObjectData.BytesHeldCount);
					//We can't return a buffer to the pool unless we're certain it's not
					//being used anywhere else, which means this cache item must be
					//being disposed.
					if (disposing)
						_bufferCache.Return(_realComposedLut.Data);

					_realComposedLut = null;
					_largeObjectData.BytesHeldCount = 0;
					_largeObjectData.LargeObjectCount = 0;
					MemoryManager.Remove(this);
				}
			}

			#region ILargeObjectContainer Members

			Guid ILargeObjectContainer.Identifier
			{
				get { return _largeObjectData.Identifier; }
			}

			int ILargeObjectContainer.LargeObjectCount
			{
				get { return _largeObjectData.LargeObjectCount; }
			}

			long ILargeObjectContainer.BytesHeldCount
			{
				get { return _largeObjectData.BytesHeldCount; }
			}

			DateTime ILargeObjectContainer.LastAccessTime
			{
				get { return _largeObjectData.LastAccessTime; }
			}

			RegenerationCost ILargeObjectContainer.RegenerationCost
			{
				get { return _largeObjectData.RegenerationCost; }
			}

			bool ILargeObjectContainer.IsLocked
			{
				get { return _largeObjectData.IsLocked; }
			}

			void ILargeObjectContainer.Lock()
			{
				_largeObjectData.Lock();
			}

			void ILargeObjectContainer.Unlock()
			{
				_largeObjectData.Unlock();
			}

			void ILargeObjectContainer.Unload()
			{
				//if (_realComposedLut != null)
				//    Trace.WriteLine(String.Format("Memory Manager unloading Composed Lut '{0}'", Key), "LUT");

				Unload(false);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					//if (_realComposedLut != null) 
					//    Trace.WriteLine(String.Format("Dispose unloading Composed Lut '{0}'", Key), "LUT");
					
					Unload(true);
					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}

			#endregion
		}

		#endregion

		#region Private Fields

		private static readonly object _syncLock = new object();
		private static readonly Dictionary<string, WeakReference> _cache = new Dictionary<string, WeakReference>();
		private static volatile bool _containsDeadItems;

		#endregion

		#region Shared Buffer Cache

		private static readonly object _bufferCacheLock = new object();
		private static volatile WeakReference _sharedBufferCache;

		private static BufferCache<int> SharedBufferCache
		{
			get
			{
				BufferCache<int> bufferCache = GetSharedBufferCache();
				if (bufferCache == null)
				{
					lock (_bufferCacheLock)
					{
						bufferCache = GetSharedBufferCache();
						if (bufferCache == null)
						{
							//Trace.WriteLine("Creating new ComposedLut.SharedBufferCache", "LUT");
							bufferCache = new BufferCache<int>(10, true);
							_sharedBufferCache = new WeakReference(bufferCache);
						}
					}
				}

				return bufferCache;
			}
		}

		private static BufferCache<int> GetSharedBufferCache()
		{
			if (_sharedBufferCache == null)
				return null;

			BufferCache<int> bufferCache;
			try
			{
				bufferCache = _sharedBufferCache.Target as BufferCache<int>;
			}
			catch (InvalidOperationException)
			{
				bufferCache = null;
			}

			return bufferCache;
		}

		#endregion

		#region Private Helper Methods

		private static string GetKey(IEnumerable<IComposableLut> luts)
		{
			return StringUtilities.Combine(luts, "/", lut => lut.GetKey());
		}

		private static void CleanupDeadItems()
		{
			if (!_containsDeadItems)
				return;

			_containsDeadItems = false;

			List<string> deadObjectKeys = new List<string>();
			foreach (KeyValuePair<string, WeakReference> pair in _cache)
			{
				try
				{
					if (!pair.Value.IsAlive || pair.Value.Target == null)
						deadObjectKeys.Add(pair.Key);
				}
				catch (InvalidOperationException)
				{
					deadObjectKeys.Add(pair.Key);
				}
			}

			foreach (string deadObjectKey in deadObjectKeys)
				_cache.Remove(deadObjectKey);

			if (_cache.Count == 0)
				Trace.WriteLine("The composed lut cache is empty.", "LUT");
		}

		private static void OnProxyDisposed(CacheItemProxy cacheItemProxy)
		{
			ReferenceCountedObjectWrapper<CacheItem> wrapper = cacheItemProxy.Wrapper;
			if (!wrapper.IsReferenceCountAboveZero())
			{
				lock (_syncLock)
				{
					//The count could have gone back up
					if (wrapper.IsReferenceCountAboveZero())
						return;

					CacheItem cacheItem = wrapper.Item;
					_cache.Remove(cacheItem.Key);
					cacheItem.Dispose();

					if (_cache.Count == 0)
						Trace.WriteLine("The composed lut cache is empty.", "LUT");
				}
			}
		}

		private static CacheItemProxy CreateItemProxy(IEnumerable<IComposableLut> luts)
		{
			string key = GetKey(luts);

			lock (_syncLock)
			{
				CleanupDeadItems();

				ReferenceCountedObjectWrapper<CacheItem> wrapper;
				if (!_cache.ContainsKey(key))
				{
					wrapper = new ReferenceCountedObjectWrapper<CacheItem>(new CacheItem(key));
					_cache[key] = new WeakReference(wrapper);
				}
				else
				{
					WeakReference reference = _cache[key];
					try
					{
						wrapper = reference.Target as ReferenceCountedObjectWrapper<CacheItem>;
					}
					catch (InvalidOperationException)
					{
						wrapper = null;
					}

					if (wrapper == null)
					{
						wrapper = new ReferenceCountedObjectWrapper<CacheItem>(new CacheItem(key));
						_cache[key] = new WeakReference(wrapper);
					}
				}

				return new CacheItemProxy(wrapper);
			}
		}

		#endregion
		#endregion
	}
}
