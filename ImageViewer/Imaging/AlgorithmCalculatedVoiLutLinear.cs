
using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class AlgorithmCalculatedVoiLutLinear : CalculatedVoiLutLinear, IVoiLutLinear
	{
		private readonly IndexedPixelData _pixelData;
		private readonly IModalityLut _modalityLut;

		private double _windowWidth;
		private double _windowCenter;

		protected AlgorithmCalculatedVoiLutLinear(IndexedPixelData pixelData, IModalityLut modalityLut)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");

			_pixelData = pixelData;
			_modalityLut = modalityLut;

			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		protected AlgorithmCalculatedVoiLutLinear(IndexedPixelData pixelData)
			: this(pixelData, null)
		{
		}

		private AlgorithmCalculatedVoiLutLinear()
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

		protected abstract void CalculateWindowRange(IndexedPixelData pixelData, out int windowStart, out int windowEnd);

		#region IVoiLutLinear Members

		public sealed override double WindowWidth
		{
			get
			{
				if (double.IsNaN(_windowWidth))
					Calculate();

				return _windowWidth;
			}
		}

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
