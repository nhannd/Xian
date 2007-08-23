using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	// LUT flyweight factory
	internal sealed class LutFactory : IReferenceCountable, IDisposable
	{
		private static volatile LutFactory _instance;
		private static object _syncRoot = new Object();

		private List<IVoiLutFactory> _voiLutFactories;
		private List<IPresentationLutFactory> _presentationLutFactories;

		private List<IModalityLut> _modalityLUTs;
		private List<IVoiLut> _voiLUTs;
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
				{
					lock (_syncRoot)
					{
						if (_instance == null)
							_instance = new LutFactory();
					}
				}

				_instance.IncrementReferenceCount();

				return _instance;
			}
		}

		private List<IModalityLut> ModalityLuts
		{
			get
			{
				if (_modalityLUTs == null)
					_modalityLUTs = new List<IModalityLut>();

				return _modalityLUTs;
			}
		}

		private List<IVoiLut> VoiLuts
		{
			get
			{
				if (_voiLUTs == null)
					_voiLUTs = new List<IVoiLut>();

				return _voiLUTs;
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

		private List<IVoiLutFactory> VoiLutFactories
		{
			get
			{
				if (_voiLutFactories == null)
				{
					_voiLutFactories = new List<IVoiLutFactory>();

					object[] extensions = (new VoiLutFactoryExtensionPoint()).CreateExtensions();
					foreach(IVoiLutFactory factory in extensions)
						_voiLutFactories.Add(factory);
				}

				return _voiLutFactories;
			}
		}

		private List<IPresentationLutFactory> PresentationLutFactories
		{
			get
			{
				if (_presentationLutFactories == null)
				{
					_presentationLutFactories = new List<IPresentationLutFactory>();

					object[] extensions = (new PresentationLutFactoryExtensionPoint()).CreateExtensions();
					foreach (IPresentationLutFactory factory in extensions)
						_presentationLutFactories.Add(factory);
				}

				return _presentationLutFactories;
			}
		}


		internal IModalityLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
		{
			IModalityLut modalityLut = this.ModalityLuts.Find
				(
					delegate(IModalityLut lut) { return lut.GetKey() == ModalityLutLinear.GetKey(bitsStored, isSigned, rescaleSlope, rescaleIntercept); } 
				);

			if (modalityLut == null)
			{
				modalityLut = new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);
				this.ModalityLuts.Add(modalityLut);
			}

			return modalityLut;
		}

		internal IVoiLut GetVoiLut(VoiLutCreationParameters creationParameters)
		{
			IVoiLut voiLut = this.VoiLuts.Find(delegate(IVoiLut lut) { return creationParameters.Equals(lut); });

			if (voiLut == null)
			{
				IVoiLutFactory voiLutFactory = this.VoiLutFactories.Find(delegate(IVoiLutFactory factory) { return factory.Name == creationParameters.FactoryName; });
				Platform.CheckForNullReference(voiLutFactory, "voiLutFactory");
				voiLut = voiLutFactory.Create(creationParameters);
				this.VoiLuts.Add(voiLut);
			}

			return voiLut;
		}

		internal IPresentationLut GetPresentationLut(PresentationLutCreationParameters creationParameters)
		{
			IPresentationLut presentationLut = this.PresentationLuts.Find(delegate(IPresentationLut lut) { return creationParameters.Equals(lut); });
			
			if (presentationLut == null)
			{
				IPresentationLutFactory presentationLutFactory = this.PresentationLutFactories.Find(delegate(IPresentationLutFactory factory) { return factory.Name == creationParameters.FactoryName; });
				Platform.CheckForNullReference(presentationLutFactory, "presentationLutFactory");
				presentationLut = presentationLutFactory.Create(creationParameters);
				this.PresentationLuts.Add(presentationLut);
			}

			return presentationLut;
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
