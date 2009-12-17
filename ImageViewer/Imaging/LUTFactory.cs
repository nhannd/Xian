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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using System.Threading;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A factory for modality luts and color maps.
	/// </summary>
	public abstract class LutFactory : IDisposable
	{
		private LutFactory()
		{
		}

		#region Abstract Methods

		/// <summary>
		/// Gets <see cref="ColorMapDescriptor"/>s that describe all the available color maps.
		/// </summary>
		public abstract IEnumerable<ColorMapDescriptor> AvailableColorMaps { get; }

		/// <summary>
		/// Factory method for grayscale color maps.
		/// </summary>
		public abstract IDataLut GetGrayscaleColorMap();

		/// <summary>
		/// Factory method that returns a new color map given the name of a <see cref="IColorMapFactory"/>.
		/// </summary>
		public abstract IDataLut GetColorMap(string name);

		/// <summary>
		/// Factory method for linear modality luts.
		/// </summary>
		public abstract IComposableLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept);

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{}
		
		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#endregion

		private class LutFactoryProxy : LutFactory
		{
			private bool _disposed;

			internal LutFactoryProxy()
			{
				_instance.OnProxyCreated();
			}

			~LutFactoryProxy()
			{
				Dispose(false);
			}

			#region ILutFactory Members

			public override IEnumerable<ColorMapDescriptor> AvailableColorMaps
			{
				get { return _instance.AvailableColorMaps; }
			}

			public override IDataLut GetGrayscaleColorMap()
			{
				return _instance.GetGrayscaleColorMap();
			}

			public override IDataLut GetColorMap(string name)
			{
				return _instance.GetColorMap(name);
			}

			public override IComposableLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
			{
				return _instance.GetModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);
			}

			#endregion

			protected override void Dispose(bool disposing)
			{
				if (_disposed)
					return;

				_disposed = true;
				_instance.OnProxyDisposed();
			}
		}

		private abstract class CacheItem : ILargeObjectContainer
		{
			private readonly LargeObjectContainerData _largeObjectData;

			private readonly object _syncLock = new object();
			private volatile IDataLut _realLut;

			protected CacheItem()
			{
				_largeObjectData = new LargeObjectContainerData(Guid.NewGuid()) { RegenerationCost = RegenerationCost.Low };
			}

			protected abstract IDataLut CreateLut();

			public IDataLut RealLut
			{
				get
				{
					IDataLut realLut = _realLut;
					if (realLut != null)
						return realLut;

					lock (_syncLock)
					{
						if (_realLut != null)
							return _realLut;

						_realLut = CreateLut();

						//Trace.WriteLine(String.Format("Creating LUT: {0}", _realLut.GetKey()));

						//just use the creation time as the "last access time", otherwise it can get expensive when called in a tight loop.
						_largeObjectData.UpdateLastAccessTime();
						_largeObjectData.BytesHeldCount = _realLut.Data.Length * sizeof(int);
						_largeObjectData.LargeObjectCount = 1;
						MemoryManager.Add(this);
						Diagnostics.OnLargeObjectAllocated(_largeObjectData.BytesHeldCount);

						return _realLut;
					}
				}
			}

			internal void Unload()
			{
				if (_realLut == null)
					return;

				lock (_syncLock)
				{
					if (_realLut == null)
						return;

					//Trace.WriteLine(String.Format("Unloading LUT: {0}", _realLut.GetKey()));

					_realLut = null;
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

			void ILargeObjectContainer.Unload()
			{
				Unload();
			}

			#endregion
		}

		#region Color Map Classes

		private class ColorMapKey : IEquatable<ColorMapKey>
		{
			internal ColorMapKey(string factoryName, int minInputValue, int maxInputValue)
			{
				FactoryName = factoryName;
				MinInputValue = minInputValue;
				MaxInputValue = maxInputValue;
			}

			public readonly string FactoryName;
			public readonly int MinInputValue;
			public readonly int MaxInputValue;

			public override int GetHashCode()
			{
				return FactoryName.GetHashCode() + 3 * MinInputValue.GetHashCode() + 5 * MaxInputValue.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj is ColorMapKey)
					return Equals((ColorMapKey)obj);

				return false;
			}

			#region IEquatable<ColorMapKey> Members

			public bool Equals(ColorMapKey other)
			{
				return FactoryName.Equals(other.FactoryName) &&
					   MinInputValue.Equals(other.MinInputValue) &&
					   MaxInputValue.Equals(other.MaxInputValue);
			}

			#endregion

			public override string ToString()
			{
				return String.Format("{0}_{1}_{2}", FactoryName, MinInputValue, MaxInputValue);
			}
		}

		private class ColorMapCacheItem : CacheItem
		{
			private readonly ColorMapKey _key;

			internal ColorMapCacheItem(ColorMapKey key) : base()
			{
				_key = key;
			}

			protected override IDataLut CreateLut()
			{
				IDataLut source = _colorMapFactories[_key.FactoryName].Create();
				source.MinInputValue = _key.MinInputValue;
				source.MaxInputValue = _key.MaxInputValue;

				return new CachedColorMap(source);
			}

			public override string ToString()
			{
				return _key.ToString();
			}
		}

		private class CachedColorMap : SimpleDataLut
		{
			internal CachedColorMap(IDataLut source)
				: base(source.MinInputValue, source.Data, 0, 0, source.GetKey(), source.GetDescription())
			{
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
		}

		[Cloneable(true)]
		private class CachedColorMapProxy : ComposableLut, IDataLut
		{
			private readonly string _factoryName;
			private int _minInputValue;
			private int _maxInputValue;

			[CloneIgnore]
			private ColorMapCacheItem _cacheItem;

			//For cloning.
			private CachedColorMapProxy()
			{
			}

			public CachedColorMapProxy(string factoryName)
			{
				_factoryName = factoryName;
			}

			private IDataLut RealColorMap
			{
				get
				{
					if (_cacheItem == null)
						_cacheItem = _instance.GetColorMapCacheItem(new ColorMapKey(_factoryName, _minInputValue, _maxInputValue));

					return _cacheItem.RealLut;
				}
			}

			public override int MinInputValue
			{
				get
				{
					return _minInputValue;
				}
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
				get
				{
					return _maxInputValue;
				}
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
				get
				{
					return RealColorMap[index];
				}
				protected set
				{
					throw new InvalidOperationException("The color map data cannot be altered.");
				}
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
		}

		#endregion

		#region Modality Lut Classes

		private class CachedModalityLutLinear : SimpleDataLut
		{
			public CachedModalityLutLinear(IDataLut source)
				: base(source.MinInputValue, source.Data,
						source.MinOutputValue, source.MaxOutputValue,
						source.GetKey(), source.GetDescription())
			{
			}
		}

		private class ModalityLutCacheItem : CacheItem
		{
			private readonly ModalityLutLinear _sourceLut;

			internal ModalityLutCacheItem(ModalityLutLinear sourceLut)
			{
				_sourceLut = sourceLut;
				_sourceLut.Clear();
			}

			protected override IDataLut CreateLut()
			{
				CachedModalityLutLinear lut = new CachedModalityLutLinear(_sourceLut);
				_sourceLut.Clear();
				return lut;
			}

			public override string ToString()
			{
				return _sourceLut.GetKey();
			}
		}

		[Cloneable(true)]
		private class CachedModalityLutProxy : ComposableLut
		{
			[CloneCopyReference]
			private readonly ModalityLutCacheItem _cacheItem;

			public CachedModalityLutProxy(ModalityLutCacheItem cacheItem)
			{
				_cacheItem = cacheItem;
			}

			//for cloning.
			private CachedModalityLutProxy()
			{
			}

			private IDataLut RealLut
			{
				get { return _cacheItem.RealLut; }
			}

			public override int MinInputValue
			{
				get { return RealLut.MinInputValue; }
				set { }
			}

			public override int MaxInputValue
			{
				get { return RealLut.MaxInputValue; }
				set { }
			}

			public override int MinOutputValue
			{
				get { return RealLut.MinOutputValue; }
				protected set { }
			}

			public override int MaxOutputValue
			{
				get { return RealLut.MaxOutputValue; }
				protected set { }
			}

			public override int this[int index]
			{
				get { return RealLut[index]; }
				protected set { throw new InvalidOperationException("The modality lut data cannot be altered."); }
			}

			public override string GetKey()
			{
				return RealLut.GetKey();
			}

			public override string GetDescription()
			{
				return RealLut.GetDescription();
			}
		}

		#endregion

		#region Private Fields

		private static readonly Dictionary<string, IColorMapFactory> _colorMapFactories;
		private static readonly List<IColorMapFactory> _sortedColorMapFactories;
		private static readonly SingletonLutFactory _instance = new SingletonLutFactory();

		#endregion

		#region Static

		static LutFactory()
		{
			_sortedColorMapFactories = CreateSortedColorMapFactories();

			_colorMapFactories = new Dictionary<string, IColorMapFactory>();
			foreach (IColorMapFactory colorMapFactory in _sortedColorMapFactories)
				_colorMapFactories[colorMapFactory.Name] = colorMapFactory;
		}

		private static List<IColorMapFactory> CreateSortedColorMapFactories()
		{
			List<IColorMapFactory> factories = new List<IColorMapFactory>();

			try
			{
				object[] extensions = new ColorMapFactoryExtensionPoint().CreateExtensions();
				foreach (IColorMapFactory factory in extensions)
				{
					if (String.IsNullOrEmpty(factory.Name))
						Platform.Log(LogLevel.Debug, "'{0}' must have a unique name", factory.GetType().FullName);
					else
						factories.Add(factory);
				}
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			factories = CollectionUtils.Sort(factories, (f1, f2) => (f1.Description ?? "").CompareTo(f2.Description ?? ""));

			factories.Insert(0, new GrayscaleColorMapFactory());
			return factories;
		}

		/// <summary>
		/// Calls <see cref="Create"/>.
		/// </summary>
		/// <remarks>This property has been deprecated; use <see cref="Create"/> instead.</remarks>
		[Obsolete("Use Create method instead.")]
		public static LutFactory NewInstance
		{
			get { return Create(); }
		}

		/// <summary>
		/// Creates a new factory instance.
		/// </summary>
		/// <remarks>
		/// You must dispose of the returned instance when you are done with it.
		/// </remarks>
		public static LutFactory Create()
		{
			return new LutFactoryProxy();
		}

		#endregion

		#region Singleton Instance

		private class SingletonLutFactory : LutFactory
		{
			private readonly object _syncLock = new object();
			private readonly Dictionary<ColorMapKey, ColorMapCacheItem> _colorMapCache;
			private readonly Dictionary<string, ModalityLutCacheItem> _modalityLutCache;
			private int _referenceCount;

			internal SingletonLutFactory()
			{
				_colorMapCache = new Dictionary<ColorMapKey, ColorMapCacheItem>();
				_modalityLutCache = new Dictionary<string, ModalityLutCacheItem>();
			}

			#region Color Maps

			/// <summary>
			/// Gets <see cref="ColorMapDescriptor"/>s that describe all the available color maps.
			/// </summary>
			public override IEnumerable<ColorMapDescriptor> AvailableColorMaps
			{
				get
				{
					//If there's only the default grayscale one, then don't return any (no point).
					if (_sortedColorMapFactories.Count == 1)
					{
						yield break;
					}
					else
					{
						foreach (IColorMapFactory factory in _sortedColorMapFactories)
							yield return ColorMapDescriptor.FromFactory(factory);
					}
				}
			}

			/// <summary>
			/// Factory method for grayscale color maps.
			/// </summary>
			public override IDataLut GetGrayscaleColorMap()
			{
				return GetColorMap(GrayscaleColorMapFactory.FactoryName);
			}

			/// <summary>
			/// Factory method that returns a new color map given the name of a <see cref="IColorMapFactory"/>.
			/// </summary>
			public override IDataLut GetColorMap(string name)
			{
				if (!_colorMapFactories.ContainsKey(name))
					throw new ArgumentException(String.Format("No Color Map factory extension exists with the name {0}.", name));

				return new CachedColorMapProxy(name);
			}

			internal ColorMapCacheItem GetColorMapCacheItem(ColorMapKey key)
			{
				lock (_syncLock)
				{
					if (_colorMapCache.ContainsKey(key))
						return _colorMapCache[key];

					ColorMapCacheItem item = new ColorMapCacheItem(key);
					_colorMapCache[key] = item;
					return item;
				}
			}


			#endregion

			#region Modality Luts

			/// <summary>
			/// Factory method for linear modality luts.
			/// </summary>
			public override IComposableLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
			{
				ModalityLutLinear modalityLut = new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);
				string key = modalityLut.GetKey();

				lock (_syncLock)
				{
					if (_modalityLutCache.ContainsKey(key))
						return new CachedModalityLutProxy(_modalityLutCache[key]);

					ModalityLutCacheItem item = new ModalityLutCacheItem(modalityLut);
					_modalityLutCache[key] = item;
					return new CachedModalityLutProxy(item);
				}
			}

			#endregion

			internal void OnProxyCreated()
			{
				Interlocked.Increment(ref _referenceCount);
			}

			internal void OnProxyDisposed()
			{
				if (Interlocked.Decrement(ref _referenceCount) <= 0)
				{
					lock (_syncLock)
					{
						Thread.VolatileWrite(ref _referenceCount, 0); //force it to zero, just in case.

						foreach (ModalityLutCacheItem item in _modalityLutCache.Values)
							item.Unload();

						foreach (ColorMapCacheItem item in _colorMapCache.Values)
							item.Unload();

						_modalityLutCache.Clear();
						_colorMapCache.Clear();

						Trace.WriteLine("LutFactory cache is empty.");
					}
				}
			}

			#endregion
		}
	}
}
