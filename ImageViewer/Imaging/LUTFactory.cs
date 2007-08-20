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

		private List<ModalityLutLinear> _modalityLUTs;
		private List<PresentationLut> _presentationLUTs;
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

		private List<ModalityLutLinear> ModalityLuts
		{
			get
			{
				if (_modalityLUTs == null)
					_modalityLUTs = new List<ModalityLutLinear>();

				return _modalityLUTs;
			}
		}

		private List<PresentationLut> PresentationLuts
		{
			get
			{
				if (_presentationLUTs == null)
					_presentationLUTs = new List<PresentationLut>();

				return _presentationLUTs;
			}
		}

		internal ModalityLutLinear GetModalityLutLinear(
			int bitsStored,
			bool isSigned,
			double rescaleSlope,
			double rescaleIntercept)
		{
			if (rescaleSlope == 0 || double.IsNaN(rescaleSlope))
				rescaleSlope = 1;

			foreach (ModalityLutLinear lut in this.ModalityLuts)
			{
				if (lut.BitsStored == bitsStored &&
					lut.IsSigned == isSigned &&
					lut.RescaleSlope == rescaleSlope &&
					lut.RescaleIntercept == rescaleIntercept)
					return lut;
			}

			ModalityLutLinear modalityLut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			this.ModalityLuts.Add(modalityLut);

			return modalityLut;
		}

		internal PresentationLut GetPresentationLut(
			int minInputValue,
			int maxInputValue,
			bool invert)
		{
			foreach (PresentationLut lut in this.PresentationLuts)
			{
				if (lut.MaxInputValue == maxInputValue &&
					lut.MinInputValue == minInputValue &&
					lut.Invert == invert)
					return lut;
			}

			PresentationLut presentationLut = new GrayscalePresentationLut(
				minInputValue, 
				maxInputValue, 
				invert);

			this.PresentationLuts.Add(presentationLut);

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
