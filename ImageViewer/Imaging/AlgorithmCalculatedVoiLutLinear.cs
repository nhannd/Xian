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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// This class provides all the base functionality for a Linear Lut where the 
	/// <see cref="WindowWidth"/> and <see cref="WindowCenter"/> are calculated via some algorithm
	/// on (an image's) <see cref="GrayscalePixelData"/>.
	/// </summary>
	/// <remarks>
	/// Inheritors must implement the <see cref="CalculateWindowRange"/> method in order to perform
	/// their calculation.  <see cref="CalculateWindowRange"/> will only be called once, after which
	/// the <see cref="WindowWidth"/> and <see cref="WindowCenter"/> values will be cached.
	/// </remarks>
	/// <seealso cref="CalculatedVoiLutLinear"/>
	[Cloneable]
	public abstract class AlgorithmCalculatedVoiLutLinear : CalculatedVoiLutLinear
	{
		#region Private Fields

		[CloneCopyReference]
		private readonly GrayscalePixelData _pixelData;
		
		//allow this to be cloned, since it will just clone the LutFactory's proxy object, anyway.
		private readonly IComposableLut _modalityLut;

		private double _windowWidth;
		private double _windowCenter;

		#endregion

		#region Protected Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on.</param>
		/// <param name="modalityLut">The modality lut to use for calculating <see cref="WindowWidth"/> and <see cref="WindowCenter"/>, if applicable.</param>
		protected AlgorithmCalculatedVoiLutLinear(GrayscalePixelData pixelData, IComposableLut modalityLut)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");

			_pixelData = pixelData;
			_modalityLut = modalityLut;

			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on.</param>
		protected AlgorithmCalculatedVoiLutLinear(GrayscalePixelData pixelData)
			: this(pixelData, null)
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected AlgorithmCalculatedVoiLutLinear(AlgorithmCalculatedVoiLutLinear source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		#endregion

		#region Private Methods

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
			_windowCenter = windowStart + _windowWidth / 2;
		}
		
		#endregion

		#region Protected Methods

		/// <summary>
		/// Called when either of <see cref="WindowWidth"/> or <see cref="WindowCenter"/> are first accessed.
		/// </summary>
		/// <remarks>
		/// Inheritors must implement this method and return the <paramref name="windowStart"/> and <paramref name="windowEnd"/> 
		/// value range that will be used to calculate the <see cref="WindowWidth"/> and <see cref="WindowCenter"/>.
		/// </remarks>
		/// <param name="pixelData">The pixel data that is to be used to calculate <paramref name="windowStart"/> and <paramref name="windowEnd"/>.</param>
		/// <param name="windowStart">returns the start value in the window range.</param>
		/// <param name="windowEnd">returns the end value in the window range.</param>
		protected abstract void CalculateWindowRange(GrayscalePixelData pixelData, out int windowStart, out int windowEnd);

		#endregion

		#region Overrides

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
