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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A singleton factory for modality luts and color maps.
	/// </summary>
	public sealed class LutFactory : IDisposable
	{
		#region Cached Color Map

		private class CachedColorMap : SimpleDataLut
		{
			public CachedColorMap(IDataLut source)
				: base(source.MinInputValue, source.Data, 0, 0, source.GetKey(), source.GetDescription())
			{
			}

			public override int MinOutputValue{
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

		#region Cached Modality Lut

		private class CachedModalityLutLinear : SimpleDataLut
		{
			public CachedModalityLutLinear(IDataLut source)
				: base(	source.MinInputValue, source.Data, 
						source.MinOutputValue, source.MaxOutputValue, 
						source.GetKey(), source.GetDescription())
			{
			}
		}

		#endregion

		#region ColorMap Proxy Class

		[Cloneable(true)]
		private class ColorMapProxy : ComposableLut, IDataLut
		{
			private readonly string _factoryName;
			private int _minInputValue;
			private int _maxInputValue;

			[CloneIgnore]
			private IDataLut _realLut;

			//For cloning.
			private ColorMapProxy()
			{
			}

			public ColorMapProxy(string factoryName)
			{
				_factoryName = factoryName;
				_realLut = null;
			}

			private IDataLut RealLut
			{
				get
				{
					if (_realLut == null)
					{
						LutFactory factory = NewInstance;
						_realLut = factory.GetRealColorMap(_factoryName, _minInputValue, _maxInputValue);
						factory.Dispose();
					}

					return _realLut;
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

					_realLut = null;
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

					_realLut = null;
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
					return this.RealLut[index];
				}
				protected set
				{
					throw new InvalidOperationException("The color map data cannot be altered.");
				}
			}

			public override string GetKey()
			{

				return this.RealLut.GetKey();
			}

			public override string GetDescription()
			{
				return this.RealLut.GetDescription();
			}

			#region IDataLut Members

			public int FirstMappedPixelValue
			{
				get { return RealLut.FirstMappedPixelValue; }
			}

			public int[] Data
			{
				get { return RealLut.Data; }
			}

			#endregion

			#region IMemorable Members

			public override object CreateMemento()
			{
				//no state to remember, but we do want to remove the reference to the 'real lut'.  It will be recreated later.
				_realLut = null;
				return base.CreateMemento();
			}

			#endregion
		}

		#endregion

		#region Modality Lut Proxy

		[Cloneable(true)]
		private class ModalityLutProxy : ComposableLut
		{
			[CloneCopyReference]
			private readonly IComposableLut _realLut;

			public ModalityLutProxy(IComposableLut realLut)
			{
				_realLut = realLut;
			}

			//for cloning.
			private ModalityLutProxy()
			{
			}

			public override int MinInputValue
			{
				get { return _realLut.MinInputValue; }
				set { }
			}

			public override int MaxInputValue
			{
				get { return _realLut.MaxInputValue; }
				set { }
			}

			public override int MinOutputValue
			{
				get { return _realLut.MinOutputValue; }
				protected set { }
			}

			public override int MaxOutputValue
			{
				get { return _realLut.MaxOutputValue; }
				protected set { }
			}

			public override int this[int index]
			{
				get { return _realLut[index]; }
				protected set { throw new InvalidOperationException("The modality lut data cannot be altered."); }
			}

			public override string GetKey()
			{

				return _realLut.GetKey();
			}

			public override string GetDescription()
			{
				return _realLut.GetDescription();
			}
		}

		#endregion

		#region Private Fields

		private static readonly object _syncLock = new object();
		private static readonly LutFactory _instance = new LutFactory();

		private readonly List<IComposableLut> _modalityLUTs = new List<IComposableLut>();
		private readonly List<IColorMapFactory> _colorMapFactories = CreateColorMapFactories();
		private readonly List<IDataLut> _colorMaps = new List<IDataLut>();
		
		private int _referenceCount = 0;

		#endregion

		private LutFactory()
		{
		}

		#region Public Properties

		/// <summary>
		/// Gets a new factory instance.
		/// </summary>
		/// <remarks>
		/// You must dispose of the returned instance when you are done with it.
		/// </remarks>
		public static LutFactory NewInstance
		{
			get
			{
				lock (_syncLock)
				{
					++_instance._referenceCount;
				}
				
				return _instance;
			}
		}

		/// <summary>
		/// Gets <see cref="ColorMapDescriptor"/>s that describe all the available color maps.
		/// </summary>
		public IEnumerable<ColorMapDescriptor> AvailableColorMaps
		{
			get
			{
				//If there's only the default grayscale one, then don't return any (no point).
				if (this.ColorMapFactories.Count == 1)
				{
					yield break;
				}
				else
				{
					foreach (IColorMapFactory factory in this.ColorMapFactories)
					{
						yield return ColorMapDescriptor.FromFactory(factory);
					}
				}
			}
		}

		#endregion

		#region Private Properties

		private List<IComposableLut> ModalityLuts
		{
			get { return _modalityLUTs; }
		}

		private List<IColorMapFactory> ColorMapFactories
		{
			get { return _colorMapFactories; }
		}

		private List<IDataLut> ColorMaps
		{
			get { return _colorMaps; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Factory method for linear modality luts.
		/// </summary>
		public IComposableLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
		{
			ModalityLutLinear modalityLut = new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);

			lock (_syncLock)
			{
				IComposableLut existingLut =
					this.ModalityLuts.Find(delegate(IComposableLut lut) { return lut.GetKey() == modalityLut.GetKey(); });

				// cache the lut (and generate it's data right away) for thread-safety
				if (existingLut == null)
					this.ModalityLuts.Add(existingLut = new CachedModalityLutLinear(modalityLut));

				return new ModalityLutProxy(existingLut);
			}
		}

		/// <summary>
		/// Factory method for grayscale color maps.
		/// </summary>
		public IDataLut GetGrayscaleColorMap()
		{
			return this.GetColorMap(GrayscaleColorMapFactory.FactoryName);
		}

		/// <summary>
		/// Factory method that returns a new color map given the name of a <see cref="IColorMapFactory"/>.
		/// </summary>
		public IDataLut GetColorMap(string name)
		{
			if (this.ColorMapFactories.Find(delegate(IColorMapFactory factory) { return factory.Name == name; }) == null)
				throw new ArgumentException(String.Format("No Color Map factory extension exists with the name {0}.", name));

			return new ColorMapProxy(name);
		}

		#endregion

		#region Private Methods

		private IDataLut GetRealColorMap(string factoryName, int minInputValue, int maxInputValue)
		{
			IColorMapFactory factory =
				this.ColorMapFactories.Find(delegate(IColorMapFactory testFactory) { return testFactory.Name == factoryName; });

			IDataLut colorMap = factory.Create();
			colorMap.MinInputValue = minInputValue;
			colorMap.MaxInputValue = maxInputValue;

			lock (_syncLock)
			{
				IDataLut existingLut = this.ColorMaps.Find(delegate(IDataLut lut) { return lut.GetKey() == colorMap.GetKey(); });

				// cache the lut (and generate it's data right away) for thread-safety
				if (existingLut == null)
					this.ColorMaps.Add(existingLut = new CachedColorMap(colorMap));

				return existingLut;
			}
		}

		private static List<IColorMapFactory> CreateColorMapFactories()
		{
			List<IColorMapFactory> factories = new List<IColorMapFactory>();

			//Add the default color map.
			factories.Add(new GrayscaleColorMapFactory());

			try
			{
				object[] extensions = new ColorMapFactoryExtensionPoint().CreateExtensions();
				foreach (IColorMapFactory factory in extensions)
					factories.Add(factory);
			}
			catch(NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			return factories;
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Disposes the factory.
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
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (_syncLock)
				{
					if (_referenceCount > 0)
						--_referenceCount;

					if (_referenceCount <= 0)
					{
						_modalityLUTs.Clear();
						_colorMaps.Clear();
					}
				}
			}
		}

		#endregion
	}
}
