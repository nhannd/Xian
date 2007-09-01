using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class MinMaxPixelCalculatedLinearLut : AlgorithmCalculatedVoiLutLinear
	{
		public MinMaxPixelCalculatedLinearLut(IndexedPixelData pixelData, IModalityLut modalityLut)
			: base(pixelData, modalityLut)
		{
		}

		public MinMaxPixelCalculatedLinearLut(IndexedPixelData pixelData)
			: base(pixelData)
		{
		}

		protected override void CalculateWindowRange(IndexedPixelData pixelData, out int windowStart, out int windowEnd)
		{
			pixelData.CalculateMinMaxPixelValue(out windowStart, out windowEnd);
		}

		public override string GetDescription()
		{
			return String.Format("W:{0} L:{1} (Min/Max)", WindowWidth, WindowCenter);
		}
	}
}
