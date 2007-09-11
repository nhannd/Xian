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
		private class PresentationLutProxy : Lut, IPresentationLut, IEquatable<PresentationLutProxy>
		{
			private readonly string _factoryName;
			private bool _invert;
			private int _minInputValue;
			private int _maxInputValue;

			private IPresentationLut _realLut;

			public PresentationLutProxy(string factoryName)
			{
				_factoryName = factoryName;
				_realLut = null;
				_invert = false;
			}

			#region IPresentationLut Members

			public bool Invert
			{
				get { return _invert; }
				set
				{
					if (value == _invert)
						return;

					_realLut = null;
					_invert = value;
					OnLutChanged();
				}
			}

			#endregion

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
				get { throw new InvalidOperationException("A Presentation LUT cannot have a minimum output value. "); }
				protected set { throw new InvalidOperationException("A Presentation LUT cannot have a minimum output value. "); }
			}

			public override int MaxOutputValue
			{
				get { throw new InvalidOperationException("A Presentation LUT cannot have a maximum output value. "); }
				protected set { throw new InvalidOperationException("A Presentation LUT cannot have a maximum output value. "); }
			}

			public override int this[int index]
			{
				get
				{
					return this.RealLut[index];
				}
				protected set
				{
					throw new InvalidOperationException("A Presentation LUT data cannot be altered. ");
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

			private IPresentationLut RealLut
			{
				get
				{
					if (_realLut == null)
					{
						LutFactory factory = LutFactory.NewInstance;
						_realLut = factory.GetRealPresentationLut(_factoryName, _minInputValue, _maxInputValue, _invert);
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
				
				if (obj is IPresentationLut)
					return this.Equals((IPresentationLut) obj);

				return false;
			}
			#region IEquatable<IPresentationLut> Members

			public bool Equals(IPresentationLut other)
			{
				if (other is PresentationLutProxy)
					return this.Equals((PresentationLutProxy)other);

				return false;
			}

			#endregion

			#region IEquatable<PresentationLutProxy> Members

			public bool Equals(PresentationLutProxy other)
			{
				return _factoryName == other._factoryName && _invert == other._invert;
			}

			#endregion
		}

		private static volatile LutFactory _instance;

		private List<ModalityLutLinear> _modalityLUTs;

		private List<IPresentationLutFactory> _presentationLutFactories;
		private List<IPresentationLut> _presentationLUTs;
		
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

		private List<IPresentationLutFactory> PresentationLutFactories
		{
			get
			{
				if (_presentationLutFactories == null)
				{
					_presentationLutFactories = new List<IPresentationLutFactory>();

					object[] factories = new PresentationLutFactoryExtensionPoint().CreateExtensions();
					foreach (object obj in factories)
					{
						if (obj is IPresentationLutFactory)
							_presentationLutFactories.Add(obj as IPresentationLutFactory);
					}
				}

				return _presentationLutFactories;
			}
		}

		private List<IPresentationLut> PresentationLuts
		{
			get
			{
				if (_presentationLUTs == null)
					_presentationLUTs = new List<IPresentationLut>();

				return _presentationLUTs;
			}
		}

		internal IEnumerable<PresentationLutDescriptor> AvailablePresentationLuts
		{
			get
			{
				foreach (IPresentationLutFactory factory in this.PresentationLutFactories)
				{
					yield return PresentationLutDescriptor.FromFactory(factory);
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

		internal IPresentationLut GetPresentationLut(string name)
		{
			if (this.PresentationLutFactories.Find(delegate(IPresentationLutFactory factory) { return factory.Name == name; }) == null)
				throw new ArgumentException(String.Format("No Presentation Lut factory extension exists with the name {0}", name));

			return new PresentationLutProxy(name);
		}

		private IPresentationLut GetRealPresentationLut(string factoryName, int minInputValue, int maxInputValue, bool invert)
		{
			IPresentationLutFactory factory =
				this.PresentationLutFactories.Find(delegate(IPresentationLutFactory testFactory) { return testFactory.Name == factoryName; });

			IPresentationLut presentationLut = factory.Create();
			presentationLut.MinInputValue = minInputValue;
			presentationLut.MaxInputValue = maxInputValue;
			presentationLut.Invert = invert;

			IPresentationLut existingLut = this.PresentationLuts.Find(delegate(IPresentationLut lut)
			                                                          	{ return lut.Equals(presentationLut); });

			if (existingLut == null)
				this.PresentationLuts.Add(existingLut = presentationLut);

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

					if (_presentationLUTs != null)
					{
						_presentationLUTs.Clear();
						_presentationLUTs = null;
					}
				}
			}
		}

		#endregion
	}
}
