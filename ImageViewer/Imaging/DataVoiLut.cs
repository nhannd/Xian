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
	/// Base implementation of a lookup table in the standard grayscale image display pipeline used to select a range from manufacturer-independent values for display implemented as an array of values.
	/// </summary>
	[Cloneable(true)]
	public abstract class DataVoiLut : DataLut, IVoiLut
	{
		double IComposableLut.MinInputValue
		{
			get { return MinInputValue; }
			set { MinInputValue = (int) Math.Round(value); }
		}

		double IComposableLut.MaxInputValue
		{
			get { return MaxInputValue; }
			set { MaxInputValue = (int) Math.Round(value); }
		}

		double IComposableLut.MinOutputValue
		{
			get { return MinOutputValue; }
		}

		double IComposableLut.MaxOutputValue
		{
			get { return MaxOutputValue; }
		}

		int IVoiLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
		}

		double IComposableLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
		}

		public new IVoiLut Clone()
		{
			return (IVoiLut) base.Clone();
		}

		IComposableLut IComposableLut.Clone()
		{
			return Clone();
		}
	}
}