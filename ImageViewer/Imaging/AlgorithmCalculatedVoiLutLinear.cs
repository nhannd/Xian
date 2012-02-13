#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		private readonly IModalityLut _modalityLut;

		private double _windowWidth;
		private double _windowCenter;

		#endregion

		#region Protected Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pixelData">The pixel data the algorithm will be run on.</param>
		/// <param name="modalityLut">The modality lut to use for calculating <see cref="WindowWidth"/> and <see cref="WindowCenter"/>, if applicable.</param>
		protected AlgorithmCalculatedVoiLutLinear(GrayscalePixelData pixelData, IModalityLut modalityLut)
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
			int minValue, maxValue;
			CalculateWindowRange(_pixelData, out minValue, out maxValue);

			double windowStart = minValue;
			double windowEnd = maxValue;

			if (_modalityLut != null)
			{
				windowStart = _modalityLut[windowStart];
				windowEnd = _modalityLut[windowEnd];
			}

			// round the window to one decimal place so it's not ridiculous
			// value is calculated anyway and thus has no significance outside of display
			var windowWidth = Math.Max(windowEnd - windowStart + 1, 1);
			_windowWidth = Math.Round(windowWidth, 1);
			_windowCenter = Math.Round(windowStart + windowWidth/2, 1);
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
