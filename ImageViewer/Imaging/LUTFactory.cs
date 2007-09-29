using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	// LUT flyweight factory
	internal sealed class LutFactory : IReferenceCountable, IDisposable
	{
		#region ColorMap Proxy Class

		private class ColorMapProxy : ComposableLut, IColorMap, IEquatable<ColorMapProxy>
		{
			private readonly string _factoryName;
			private int _minInputValue;
			private int _maxInputValue;

			private IColorMap _realLut;

			public ColorMapProxy(string factoryName)
			{
				_factoryName = factoryName;
				_realLut = null;
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
				get { throw new InvalidOperationException(SR.ExceptionColorMapCannotHaveMinimumOutputValue); }
				protected set { throw new InvalidOperationException(SR.ExceptionColorMapCannotHaveMinimumOutputValue); }
			}

			public override int MaxOutputValue
			{
				get { throw new InvalidOperationException(SR.ExceptionColorMapCannotHaveMaximumOutputValue); }
				protected set { throw new InvalidOperationException(SR.ExceptionColorMapCannotHaveMaximumOutputValue); }
			}

			public override int this[int index]
			{
				get
				{
					return this.RealLut[index];
				}
				protected set
				{
					throw new InvalidOperationException(SR.ExceptionColorMapDataCannotBeAltered);
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

			#region IColorMap Members

			public int[] Data
			{
				get { return RealLut.Data; }
			}

			#endregion

			private IColorMap RealLut
			{
				get
				{
					if (_realLut == null)
					{
						LutFactory factory = LutFactory.NewInstance;
						_realLut = factory.GetRealColorMap(_factoryName, _minInputValue, _maxInputValue);
						factory.Dispose();
					}

					return _realLut;
				}
			}

			#region IMemorable Members

			public override IMemento CreateMemento()
			{
				//no state to remember, but we do want to remove the reference to the 'real lut'.  It will be recreated later.
				_realLut = null;
				return base.CreateMemento();
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (this == obj)
					return true;
				
				if (obj is IColorMap)
					return this.Equals((IColorMap) obj);

				return false;
			}
			#region IEquatable<IColorMap> Members

			public bool Equals(IColorMap other)
			{
				if (other is ColorMapProxy)
					return this.Equals((ColorMapProxy)other);

				return false;
			}

			#endregion

			#region IEquatable<ColorMapProxy> Members

			public bool Equals(ColorMapProxy other)
			{
				return _factoryName == other._factoryName;
			}

			#endregion
		}

		#endregion

		private static volatile LutFactory _instance;

		private List<ModalityLutLinear> _modalityLUTs;

		private List<IColorMapFactory> _colorMapFactories;
		private List<IColorMap> _colorMaps;
		
		private int _referenceCount = 0;

		private LutFactory()
		{

		}

		public static LutFactory NewInstance
		{
			get
			{
				if (_instance == null)
					_instance = new LutFactory();

				_instance.IncrementReferenceCount();
				return _instance;
			}
		}

		private List<ModalityLutLinear> ModalityLuts
		{
			get
			{
				if (_modalityLUTs == null)
					_modalityLUTs = new List<ModalityLutLinear>();

				return _modalityLUTs;
			}
		}

		private List<IColorMapFactory> ColorMapFactories
		{
			get
			{
				if (_colorMapFactories == null)
				{
					_colorMapFactories = new List<IColorMapFactory>();

					object[] factories = new ColorMapFactoryExtensionPoint().CreateExtensions();
					foreach (object obj in factories)
					{
						if (obj is IColorMapFactory)
							_colorMapFactories.Add(obj as IColorMapFactory);
					}
				}

				return _colorMapFactories;
			}
		}

		private List<IColorMap> ColorMaps
		{
			get
			{
				if (_colorMaps == null)
					_colorMaps = new List<IColorMap>();

				return _colorMaps;
			}
		}

		internal IEnumerable<ColorMapDescriptor> AvailableColorMaps
		{
			get
			{
				foreach (IColorMapFactory factory in this.ColorMapFactories)
				{
					yield return ColorMapDescriptor.FromFactory(factory);
				}
			}
		}

		internal ModalityLutLinear GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
		{
			ModalityLutLinear modalityLut = new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);

			ModalityLutLinear existingLut = this.ModalityLuts.Find(delegate(ModalityLutLinear lut) { return lut.Equals(modalityLut); });

			if (existingLut == null)
				this.ModalityLuts.Add(existingLut = modalityLut);

			return existingLut;
		}

		internal IColorMap GetColorMap(string name)
		{
			if (this.ColorMapFactories.Find(delegate(IColorMapFactory factory) { return factory.Name == name; }) == null)
				throw new ArgumentException(String.Format(SR.ExceptionFormatNoColorMapFactoryExistWithName, name));

			return new ColorMapProxy(name);
		}

		private IColorMap GetRealColorMap(string factoryName, int minInputValue, int maxInputValue)
		{
			IColorMapFactory factory =
				this.ColorMapFactories.Find(delegate(IColorMapFactory testFactory) { return testFactory.Name == factoryName; });

			IColorMap colorMap = factory.Create();
			colorMap.MinInputValue = minInputValue;
			colorMap.MaxInputValue = maxInputValue;

			IColorMap existingLut = this.ColorMaps.Find(delegate(IColorMap lut){ return lut.Equals(colorMap); });

			if (existingLut == null)
				this.ColorMaps.Add(existingLut = colorMap);

			return existingLut;
		}

		#region IReferenceCountable Members

		public void IncrementReferenceCount()
		{
			_referenceCount++;
		}

		public void DecrementReferenceCount()
		{
			if (_referenceCount > 0)
				_referenceCount--;
		}

		public bool IsReferenceCountZero
		{
			get { return _referenceCount == 0; }
		}

		public int ReferenceCount
		{
			get { return _referenceCount; }
		}

		#endregion


		#region Disposal

		#region IDisposable Members

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

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DecrementReferenceCount();

				if (this.IsReferenceCountZero)
				{
					if (_modalityLUTs != null)
					{
						_modalityLUTs.Clear();
						_modalityLUTs = null;
					}

					if (_colorMaps != null)
					{
						_colorMaps.Clear();
						_colorMaps = null;
					}
				}
			}
		}

		#endregion
	}
}
