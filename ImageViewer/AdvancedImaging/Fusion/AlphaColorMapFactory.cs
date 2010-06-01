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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public static class AlphaColorMapFactory
	{
		private static readonly Dictionary<ICachedColorMapKey, CacheItem> _cache = new Dictionary<ICachedColorMapKey, CacheItem>();
		private static readonly object _syncLock = new object();

		public static IDataLut GetColorMap(string colorMapName, byte alpha, bool hideBackground)
		{
			return new CachedColorMapProxy(new AlphaColorMapKey(colorMapName, alpha, hideBackground));
		}

		private static CacheItem GetColorMapCacheItem(ICachedColorMapKey key)
		{
			lock (_syncLock)
			{
				if (_cache.ContainsKey(key))
					return _cache[key];
				CacheItem item = new CacheItem(key);
				_cache[key] = item;
				return item;
			}
		}

		#region ICachedColorMapKey Interface

		private interface ICachedColorMapKey
		{
			IDataLut CreateColorMap();
		}

		#endregion

		#region AlphaColorMapKey Class

		[Cloneable(true)]
		private class AlphaColorMapKey : ICachedColorMapKey, IEquatable<AlphaColorMapKey>
		{
			private readonly string _colorMapName;
			private readonly byte _alpha;
			private readonly bool _hideBackground;

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private AlphaColorMapKey() {}

			public AlphaColorMapKey(string colorMapName, byte alpha, bool hideBackground)
				: this()
			{
				_colorMapName = colorMapName;
				_alpha = alpha;
				_hideBackground = hideBackground;
			}

			public IDataLut CreateColorMap()
			{
				IDataLut baseColorMap;
				if (_colorMapName == "HOT_IRON")
				{
					baseColorMap = new HotIronColorMapFactory().Create();
				}
				else
				{
					using (LutFactory lutFactory = LutFactory.Create())
					{
						baseColorMap = lutFactory.GetColorMap(_colorMapName);
					}
				}

				return new AlphaColorMap(baseColorMap, _alpha, _hideBackground);
			}

			public override int GetHashCode()
			{
				return 0x15BDF4E1 ^ _colorMapName.GetHashCode() ^ _alpha.GetHashCode() ^ _hideBackground.GetHashCode();
			}

			public bool Equals(AlphaColorMapKey other)
			{
				return _colorMapName.Equals(other._colorMapName) && _alpha.Equals(other._alpha) && _hideBackground.Equals(other._hideBackground);
			}

			public override bool Equals(object obj)
			{
				if (obj is AlphaColorMapKey)
					return Equals((AlphaColorMapKey) obj);
				return false;
			}

			public override string ToString()
			{
				return String.Format("{0}[alpha={1},hidebkg={2}]", _colorMapName, _alpha, _hideBackground ? 1 : 0);
			}
		}

		#endregion

		#region CachedColorMapProxy Class

		[Cloneable(true)]
		private class CachedColorMapProxy : ComposableLut, IDataLut
		{
			private readonly ICachedColorMapKey _colorMapKey;
			private int _minInputValue;
			private int _maxInputValue;

			[CloneIgnore]
			private CacheItem _cacheItem;

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private CachedColorMapProxy() {}

			public CachedColorMapProxy(ICachedColorMapKey colorMapKey)
				: this()
			{
				_colorMapKey = colorMapKey;
			}

			private IDataLut RealColorMap
			{
				get
				{
					if (_cacheItem == null)
						_cacheItem = GetColorMapCacheItem(new FullColorMapKey(_colorMapKey, _minInputValue, _maxInputValue));
					return _cacheItem.RealColorMap;
				}
			}

			public override int MinInputValue
			{
				get { return _minInputValue; }
				set
				{
					if (value == _minInputValue)
						return;

					_cacheItem = null;
					_minInputValue = value;
					OnLutChanged();
				}
			}

			public override int MaxInputValue
			{
				get { return _maxInputValue; }
				set
				{
					if (value == _maxInputValue)
						return;

					_cacheItem = null;
					_maxInputValue = value;
					OnLutChanged();
				}
			}

			public override int MinOutputValue
			{
				get { throw new InvalidOperationException("A color map cannot have a minimum output value."); }
				protected set { throw new InvalidOperationException("A color map cannot have a minimum output value."); }
			}

			public override int MaxOutputValue
			{
				get { throw new InvalidOperationException("A color map cannot have a maximum output value."); }
				protected set { throw new InvalidOperationException("A color map cannot have a maximum output value."); }
			}

			public override int this[int index]
			{
				get { return RealColorMap[index]; }
				protected set { throw new InvalidOperationException("The color map data cannot be altered."); }
			}

			public override string GetKey()
			{
				return RealColorMap.GetKey();
			}

			public override string GetDescription()
			{
				return RealColorMap.GetDescription();
			}

			#region IDataLut Members

			public int FirstMappedPixelValue
			{
				get { return RealColorMap.FirstMappedPixelValue; }
			}

			public int[] Data
			{
				get { return RealColorMap.Data; }
			}

			#endregion

			#region IMemorable Members

			public override object CreateMemento()
			{
				//no state to remember, but we do want to remove the reference to the 'real lut'.  It will be recreated later.
				_cacheItem = null;
				return base.CreateMemento();
			}

			#endregion

			#region FullColorMapKey<T> Class

			[Cloneable(true)]
			private class FullColorMapKey : ICachedColorMapKey, IEquatable<FullColorMapKey>
			{
				private readonly ICachedColorMapKey _colorMapKey;
				private readonly int _minInputValue;
				private readonly int _maxInputValue;

				/// <summary>
				/// Cloning constructor.
				/// </summary>
				private FullColorMapKey() {}

				public FullColorMapKey(ICachedColorMapKey colorMapKey, int minInputValue, int maxInputValue)
					: this()
				{
					_colorMapKey = colorMapKey;
					_minInputValue = minInputValue;
					_maxInputValue = maxInputValue;
				}

				public IDataLut CreateColorMap()
				{
					var colorMap = _colorMapKey.CreateColorMap();
					colorMap.MinInputValue = _minInputValue;
					colorMap.MaxInputValue = _maxInputValue;
					return colorMap;
				}

				public override int GetHashCode()
				{
					return 0x152D6351 ^ _colorMapKey.GetHashCode() ^ _minInputValue.GetHashCode() ^ _maxInputValue.GetHashCode();
				}

				public bool Equals(FullColorMapKey other)
				{
					return _colorMapKey.Equals(other._colorMapKey) && _minInputValue.Equals(other._minInputValue) && _maxInputValue.Equals(other._maxInputValue);
				}

				public override bool Equals(object obj)
				{
					if (obj is FullColorMapKey)
						return Equals((FullColorMapKey) obj);
					return false;
				}

				public override string ToString()
				{
					return string.Format("{0}[RangeIn:{1},{2}]", _colorMapKey, _minInputValue, _maxInputValue);
				}
			}

			#endregion
		}

		#endregion

		#region CacheItem Class

		private class CacheItem : ILargeObjectContainer
		{
			private readonly object _syncLock = new object();
			private readonly LargeObjectContainerData _largeObjectData = new LargeObjectContainerData(Guid.NewGuid()) {RegenerationCost = RegenerationCost.Low};
			private volatile IDataLut _realColorMap;

			private readonly ICachedColorMapKey _key;

			internal CacheItem(ICachedColorMapKey key)
			{
				_key = key;
			}

			public override string ToString()
			{
				return _key.ToString();
			}

			public IDataLut RealColorMap
			{
				get
				{
					IDataLut realLut = _realColorMap;
					if (realLut != null)
						return realLut;

					lock (_syncLock)
					{
						if (_realColorMap != null)
							return _realColorMap;
						_realColorMap = new CompiledColorMap(_key.CreateColorMap());

						//just use the creation time as the "last access time", otherwise it can get expensive when called in a tight loop.
						_largeObjectData.UpdateLastAccessTime();
						_largeObjectData.BytesHeldCount = _realColorMap.Data.Length*sizeof (int);
						_largeObjectData.LargeObjectCount = 1;

						MemoryManager.Add(this);
						Diagnostics.OnLargeObjectAllocated(_largeObjectData.BytesHeldCount);
						return _realColorMap;
					}
				}
			}

			public void Unload()
			{
				if (_realColorMap == null)
					return;

				lock (_syncLock)
				{
					if (_realColorMap == null)
						return;
					_realColorMap = null;

					Diagnostics.OnLargeObjectReleased(_largeObjectData.BytesHeldCount);
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

			#endregion

			#region CompiledColorMap Class

			private class CompiledColorMap : SimpleDataLut
			{
				internal CompiledColorMap(IDataLut source)
					: base(source.MinInputValue, source.Data, 0, 0, source.GetKey(), source.GetDescription()) {}

				public override int MinOutputValue
				{
					get { throw new InvalidOperationException("A color map cannot have a minimum output value."); }
					protected set { throw new InvalidOperationException("A color map cannot have a minimum output value."); }
				}

				public override int MaxOutputValue
				{
					get { throw new InvalidOperationException("A color map cannot have a maximum output value."); }
					protected set { throw new InvalidOperationException("A color map cannot have a maximum output value."); }
				}
			}

			#endregion
		}

		#endregion
	}
}