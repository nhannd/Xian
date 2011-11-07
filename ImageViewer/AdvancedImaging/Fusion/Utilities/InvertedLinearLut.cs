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
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Utilities
{
	[Cloneable]
	internal sealed class InvertedLinearLut : ComposableLut
	{
		[CloneIgnore]
		private readonly double _rescaleSlope;

		[CloneIgnore]
		private readonly double _rescaleIntercept;

		/// <summary>
		/// Initializes a new instance of <see cref="NormalizationLutLinear"/>.
		/// </summary>
		/// <param name="rescaleSlope">Original frame rescale slope to be inverted.</param>
		/// <param name="rescaleIntercept">Original frame rescale intercept to be inverted.</param>
		public InvertedLinearLut(double rescaleSlope, double rescaleIntercept)
		{
			_rescaleSlope = rescaleSlope;
			_rescaleIntercept = rescaleIntercept;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private InvertedLinearLut(InvertedLinearLut source, ICloningContext context)
		{
			_rescaleSlope = source._rescaleSlope;
			_rescaleIntercept = source._rescaleIntercept;
		}

		public override double MinInputValue { get; set; }

		public override double MaxInputValue { get; set; }

		public override double MinOutputValue
		{
			get { return this[MinInputValue]; }
			protected set { throw new NotSupportedException(); }
		}

		public override double MaxOutputValue
		{
			get { return this[MaxInputValue]; }
			protected set { throw new NotSupportedException(); }
		}

		public override double this[double input]
		{
			get { return (input - _rescaleIntercept)/_rescaleSlope; }
		}

		public override string GetKey()
		{
			return string.Format(@"NormInverse_{0}_{1}", _rescaleSlope, _rescaleIntercept);
		}

		public override string GetDescription()
		{
			return string.Format(@"Normalization Function Inverse of m={0} b={1}", _rescaleSlope, _rescaleIntercept);
		}
	}
}