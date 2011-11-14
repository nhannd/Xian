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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An implementation of a VOI LUT that represents the identity function for integer inputs between <see cref="MinInputValue"/> and <see cref="MaxInputValue"/>.
	/// </summary>
	[Cloneable(true)]
	public sealed class IdentityVoiLinearLut : ComposableVoiLut, IVoiLutLinear
	{
		/// <summary>
		/// Initializes a new instance of <see cref="IdentityVoiLinearLut"/>.
		/// </summary>
		public IdentityVoiLinearLut()
		{
			MinInputValue = int.MinValue;
			MaxInputValue = int.MaxValue;
		}

#if UNIT_TESTS

		internal IdentityVoiLinearLut(int bitsStored, bool signed)
		{
			MinInputValue = DicomPixelData.GetMinPixelValue(bitsStored, signed);
			MaxInputValue = DicomPixelData.GetMaxPixelValue(bitsStored, signed);
		}

#endif

		/// <summary>
		/// Gets an abbreviated description of the lookup table.
		/// </summary>
		public override string GetDescription()
		{
			return string.Format(SR.FormatDescriptionIdentityVoiLinearLut, WindowWidth, WindowCenter);
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public override double MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public override double MaxInputValue { get; set; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		/// <remarks>
		/// This will always return <see cref="MinInputValue"/> rounded to an integer.
		/// </remarks>
		public override int MinOutputValue
		{
			get { return (int) Math.Round(MinInputValue); }
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		/// <remarks>
		/// This will always return <see cref="MaxInputValue"/> rounded to an integer.
		/// </remarks>
		public override int MaxOutputValue
		{
			get { return (int) Math.Round(MaxInputValue); }
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		public override int this[double input]
		{
			get
			{
				if (input < MinInputValue)
					return MinOutputValue;
				if (input > MaxInputValue)
					return MaxOutputValue;
				return (int) Math.Round(input);
			}
		}

		/// <summary>
		/// Gets the window width.
		/// </summary>
		/// <remarks>
		/// This value is always exactly <see cref="MaxInputValue"/>-<see cref="MinInputValue"/>+1.
		/// </remarks>
		public double WindowWidth
		{
			get { return MaxInputValue - MinInputValue + 1; }
		}

		/// <summary>
		/// Gets the window centre.
		/// </summary>
		/// <remarks>
		/// This value is always exactly (<see cref="MaxInputValue"/>-<see cref="MinInputValue"/>+1)/2.
		/// </remarks>
		public double WindowCenter
		{
			get { return WindowWidth/2; }
		}

		/// <summary>
		/// Gets a string key that identifies this particular lookup table's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some lookup tables can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		public override string GetKey()
		{
			return string.Format("IDENTITY_{0}_to_{1}", MinInputValue, MaxInputValue);
		}
	}
}