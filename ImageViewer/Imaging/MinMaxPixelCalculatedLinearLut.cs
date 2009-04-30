#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
		/// The input <see cref="IComposableLut"/> object can be null.
		/// </remarks>
		/// <param name="pixelData">The pixel data the algorithm will be run on.</param>
		/// <param name="modalityLut">The modality lut to use for calculating <see cref="AlgorithmCalculatedVoiLutLinear.WindowWidth"/> 
		/// and <see cref="AlgorithmCalculatedVoiLutLinear.WindowCenter"/>, if applicable.</param>
		public MinMaxPixelCalculatedLinearLut(GrayscalePixelData pixelData, IComposableLut modalityLut)
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
