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
	[Cloneable]
	internal sealed class PETLinearNormalizationLut : ComposableLut
	{
		[CloneIgnore]
		private readonly double _inverseSlope;

		[CloneIgnore]
		private readonly double _inverseIntercept;

		/// <summary>
		/// Initializes a new instance of <see cref="PETLinearNormalizationLut"/>.
		/// </summary>
		/// <param name="rescaleSlope">Original frame rescale slope to be inverted.</param>
		/// <param name="rescaleIntercept">Original frame rescale intercept to be inverted.</param>
		public PETLinearNormalizationLut(double rescaleSlope, double rescaleIntercept)
		{
			// {y = mx + b} => {x = (1/m)b + (-b/m)}
			_inverseSlope = 1/rescaleSlope;
			_inverseIntercept = -rescaleIntercept/rescaleSlope;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private PETLinearNormalizationLut(PETLinearNormalizationLut source, ICloningContext context)
		{
			_inverseSlope = source._inverseSlope;
			_inverseIntercept = source._inverseIntercept;
		}

		public override double MinInputValue { get; set; }

		public override double MaxInputValue { get; set; }

		public override double MinOutputValue
		{
			get { return Lookup(MinInputValueCore); }
			protected set { throw new NotSupportedException(); }
		}

		public override double MaxOutputValue
		{
			get { return Lookup(MaxInputValueCore); }
			protected set { throw new NotSupportedException(); }
		}

		public override double this[double input]
		{
			get { return input*_inverseSlope + _inverseIntercept; }
		}

		public override string GetKey()
		{
			return string.Format(@"PETNorm_{0}_{1}", _inverseSlope, _inverseIntercept);
		}

		public override string GetDescription()
		{
			return string.Format(@"PET Normalization Function m={0} b={1}", _inverseSlope, _inverseIntercept);
		}
	}
}