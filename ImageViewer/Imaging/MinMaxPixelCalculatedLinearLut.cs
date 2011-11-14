#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Linear Lut whose <see cref="AlgorithmCalculatedVoiLutLinear.WindowWidth"/> and <see cref="AlgorithmCalculatedVoiLutLinear.WindowCenter"/> 
	/// are calculated based on the minimum and maximum pixel value in the pixel data.
	/// </summary>
	/// <seealso cref="AlgorithmCalculatedVoiLutLinear"/>
	[Cloneable]
	public sealed class MinMaxPixelCalculatedLinearLut : AlgorithmCalculatedVoiLutLinear
	{
		#region Public Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// The input <see cref="IVoiLut"/> object can be null.
		/// </remarks>
		/// <param name="pixelData">The pixel data the algorithm will be run on.</param>
		/// <param name="modalityLut">The modality lut to use for calculating <see cref="AlgorithmCalculatedVoiLutLinear.WindowWidth"/> 
		/// and <see cref="AlgorithmCalculatedVoiLutLinear.WindowCenter"/>, if applicable.</param>
		public MinMaxPixelCalculatedLinearLut(GrayscalePixelData pixelData, IModalityLut modalityLut)
			: base(pixelData, modalityLut)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on.</param>
		public MinMaxPixelCalculatedLinearLut(GrayscalePixelData pixelData)
			: base(pixelData)
		{
		}

		private MinMaxPixelCalculatedLinearLut(MinMaxPixelCalculatedLinearLut source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Calculates and returns the minimum and maximum pixel values in the input <paramref name="pixelData"/>.
		/// </summary>
		/// <param name="pixelData">The input pixel data.</param>
		/// <param name="windowStart">Returns the minimum pixel value.</param>
		/// <param name="windowEnd">Returns the maximum pixel value.</param>
		protected override void CalculateWindowRange(GrayscalePixelData pixelData, out int windowStart, out int windowEnd)
		{
			pixelData.CalculateMinMaxPixelValue(out windowStart, out windowEnd);
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionMinMaxCalculatedLinearLut, WindowWidth, WindowCenter);
		}

		#endregion
	}
}
