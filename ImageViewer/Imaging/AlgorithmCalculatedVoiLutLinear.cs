
using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class.  This class provides all the base functionality for a Linear Lut where the 
	/// <see cref="WindowWidth"/> and <see cref="WindowCenter"/> are calculated via some algorithm
	/// on (an image's) <see cref="IndexedPixelData"/>.
	/// </summary>
	/// <remarks>
	/// Inheritors must implement the <see cref="CalculateWindowRange"/> method in order to perform
	/// their calculation.  <see cref="CalculateWindowRange"/> will only be called once, after which
	/// the <see cref="WindowWidth"/> and <see cref="WindowCenter"/> values will be cached.
	/// </remarks>
	public abstract class AlgorithmCalculatedVoiLutLinear : CalculatedVoiLutLinear
	{
		private readonly IndexedPixelData _pixelData;
		private readonly IModalityLut _modalityLut;

		private double _windowWidth;
		private double _windowCenter;

		/// <summary>
		/// Constructor.  The input modalityLut object can be null.
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on</param>
		/// <param name="modalityLut">The modality lut to use for calculating <see cref="WindowWidth"/> and <see cref="WindowCenter"/>, if applicable</param>
		protected AlgorithmCalculatedVoiLutLinear(IndexedPixelData pixelData, IModalityLut modalityLut)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");

			_pixelData = pixelData;
			_modalityLut = modalityLut;

			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on</param>
		protected AlgorithmCalculatedVoiLutLinear(IndexedPixelData pixelData)
			: this(pixelData, null)
		{
		}

		private void Calculate()
		{
			int windowStart, windowEnd;
			CalculateWindowRange(_pixelData, out windowStart, out windowEnd);

			if (_modalityLut != null)
			{
				windowStart = _modalityLut[windowStart];
				windowEnd = _modalityLut[windowEnd];
			}

			_windowWidth = (windowEnd - windowStart) + 1;
			_windowWidth = Math.Max(_windowWidth, 1);
			_windowCenter = _windowWidth / 2;
		}

		/// <summary>
		/// Called by the base class (<see cref="AlgorithmCalculatedVoiLutLinear"/>) when either of <see cref="WindowWidth"/>
		/// or <see cref="WindowCenter"/> are first accessed.  Inheritors must implement this method and return the 
		/// windowStart and windowEnd value range that will be used to calculate the <see cref="WindowWidth"/>
		/// and <see cref="WindowCenter"/>.
		/// </summary>
		/// <param name="pixelData">The pixel data that is to be used to calculate windowStart and windowEnd</param>
		/// <param name="windowStart">returns the beginning value in the window range</param>
		/// <param name="windowEnd">returns the end value in the window range</param>
		protected abstract void CalculateWindowRange(IndexedPixelData pixelData, out int windowStart, out int windowEnd);

		#region IVoiLutLinear Members

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		public sealed override double WindowWidth
		{
			get
			{
				if (double.IsNaN(_windowWidth))
					Calculate();

				return _windowWidth;
			}
		}

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		public sealed override double WindowCenter
		{
			get
			{
				if (double.IsNaN(_windowCenter))
					Calculate();

				return _windowCenter;
			}
		}

		#endregion
	}
}
