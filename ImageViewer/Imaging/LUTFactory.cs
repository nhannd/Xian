using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	// LUT flyweight factory
	internal sealed class LUTFactory : IReferenceCountable, IDisposable
	{
		private static volatile LUTFactory _instance;
		private static object _syncRoot = new Object();

		private List<ModalityLUTLinear> _modalityLUTs;
		private List<PresentationLUT> _presentationLUTs;
		private int _referenceCount = 0;

		private LUTFactory()
		{

		}

		public static LUTFactory NewInstance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncRoot)
					{
						if (_instance == null)
							_instance = new LUTFactory();
					}
				}

				_instance.IncrementReferenceCount();

				return _instance;
			}
		}

		private List<ModalityLUTLinear> ModalityLUTs
		{
			get
			{
				if (_modalityLUTs == null)
					_modalityLUTs = new List<ModalityLUTLinear>();

				return _modalityLUTs;
			}
		}

		private List<PresentationLUT> PresentationLUTs
		{
			get
			{
				if (_presentationLUTs == null)
					_presentationLUTs = new List<PresentationLUT>();

				return _presentationLUTs;
			}
		}

		internal ModalityLUTLinear GetModalityLUTLinear(
			int bitsStored,
			int pixelRepresentation,
			double rescaleSlope,
			double rescaleIntercept)
		{
			if (rescaleSlope == 0 || double.IsNaN(rescaleSlope))
				rescaleSlope = 1;

			foreach (ModalityLUTLinear lut in this.ModalityLUTs)
			{
				if (lut.BitsStored == bitsStored &&
					lut.PixelRepresentation == pixelRepresentation &&
					lut.RescaleSlope == rescaleSlope &&
					lut.RescaleIntercept == rescaleIntercept)
					return lut;
			}

			ModalityLUTLinear modalityLut = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			this.ModalityLUTs.Add(modalityLut);

			return modalityLut;
		}

		internal PresentationLUT GetPresentationLUT(
			int minInputValue,
			int maxInputValue,
			PhotometricInterpretation photometricInterpretation)
		{
			foreach (PresentationLUT lut in this.PresentationLUTs)
			{
				if (lut.MaxInputValue == maxInputValue &&
					lut.MinInputValue == minInputValue &&
					lut.PhotometricInterpretation == photometricInterpretation)
					return lut;
			}

			PresentationLUT presentationLut = new PresentationLUT(
				minInputValue, 
				maxInputValue, 
				photometricInterpretation);

			this.PresentationLUTs.Add(presentationLut);

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
				Platform.Log(e);
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
