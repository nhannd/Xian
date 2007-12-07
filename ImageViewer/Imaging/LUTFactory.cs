#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
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

			private IColorMap RealLut
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
				get { throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMinimumOutputValue); }
				protected set { throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMinimumOutputValue); }
			}

			public override int MaxOutputValue
			{
				get { throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMaximumOutputValue); }
				protected set { throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMaximumOutputValue); }
			}

			public override int this[int index]
			{
				get
				{
					return this.RealLut[index];
				}
				protected set
				{
					throw new MemberAccessException(SR.ExceptionColorMapDataCannotBeAltered);
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

			#region IMemorable Members

			public override IMemento CreateMemento()
			{
				//no state to remember, but we do want to remove the reference to the 'real lut'.  It will be recreated later.
				_realLut = null;
				return base.CreateMemento();
			}

			#endregion

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
			
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

		#region Private Fields

		private static volatile LutFactory _instance;

		private List<ModalityLutLinear> _modalityLUTs;

		private List<IColorMapFactory> _colorMapFactories;
		private List<IColorMap> _colorMaps;
		
		private int _referenceCount = 0;

		#endregion

		private LutFactory()
		{

		}

		#region Public Properties

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

		#endregion

		#region Private Properties

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

					//Add the default color map.
					_colorMapFactories.Add(new GrayscaleColorMapFactory());

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

		#region Internal Methods

		internal ModalityLutLinear GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
		{
			ModalityLutLinear modalityLut = new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);

			ModalityLutLinear existingLut = this.ModalityLuts.Find(delegate(ModalityLutLinear lut) { return lut.Equals(modalityLut); });

			if (existingLut == null)
				this.ModalityLuts.Add(existingLut = modalityLut);

			return existingLut;
		}

		internal IColorMap GetGrayscaleColorMap()
		{
			return this.GetColorMap(GrayscaleColorMapFactory.FactoryName);
		}
		
		internal IColorMap GetColorMap(string name)
		{
			if (this.ColorMapFactories.Find(delegate(IColorMapFactory factory) { return factory.Name == name; }) == null)
				throw new ArgumentException(String.Format(SR.ExceptionFormatNoColorMapFactoryExistWithName, name));

			return new ColorMapProxy(name);
		}

		#endregion

		#region Private Methods

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

		#endregion

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
