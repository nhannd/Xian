using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Linear Lut whose <see cref="AlgorithmCalculatedVoiLutLinear.WindowWidth"/> and <see cref="AlgorithmCalculatedVoiLutLinear.WindowCenter"/> 
	/// are calculated based on the minimum and maximum pixel value in the pixel data.
	/// </summary>
	public sealed class MinMaxPixelCalculatedLinearLut : AlgorithmCalculatedVoiLutLinear
	{
		/// <summary>
		/// Constructor.  The input <see cref="IModalityLut"/> object can be null.
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on</param>
		/// <param name="modalityLut">The modality lut to use for calculating <see cref="AlgorithmCalculatedVoiLutLinear.WindowWidth"/> 
		/// and <see cref="AlgorithmCalculatedVoiLutLinear.WindowCenter"/>, if applicable</param>
		public MinMaxPixelCalculatedLinearLut(IndexedPixelData pixelData, IModalityLut modalityLut)
			: base(pixelData, modalityLut)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on</param>
		public MinMaxPixelCalculatedLinearLut(IndexedPixelData pixelData)
			: base(pixelData)
		{
		}

		/// <summary>
		/// Calculates and returns the minimum and maximum pixel values in the input <see cref="IndexedPixelData"/>.
		/// </summary>
		/// <param name="pixelData">The input pixel data</param>
		/// <param name="windowStart">returns the minimum pixel value</param>
		/// <param name="windowEnd">returns the maximum pixel value</param>
		protected override void CalculateWindowRange(IndexedPixelData pixelData, out int windowStart, out int windowEnd)
		{
			pixelData.CalculateMinMaxPixelValue(out windowStart, out windowEnd);
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return String.Format("W:{0} L:{1} (Min/Max)", WindowWidth, WindowCenter);
		}
	}
}
