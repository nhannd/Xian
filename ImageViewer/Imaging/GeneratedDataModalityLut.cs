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
	/// Abstract class providing the base implementation for Data Luts that are purely generated.
	/// </summary>
	/// <remarks>
	/// Often, Linear Luts are created by deriving from this class to improve performance so that
	/// the calculation is only performed once.  For an example, see <see cref="ModalityLutLinear"/>.
	/// </remarks>
	/// <seealso cref="DataModalityLut"/>
	[Cloneable(true)]
	public abstract class GeneratedDataModalityLut : DataModalityLut, IModalityLut
	{
		[CloneIgnore]
		private double[] _data; //data will be re-generated.

		/// <summary>
		/// Since the data lut is generated, simply returns <see cref="IComposableLut.MinInputValue"/>.
		/// </summary>
		public override sealed int FirstMappedPixelValue
		{
			get { return base.MinInputValue; }
		}

		/// <summary>
		/// Since the data lut is generated, simply returns <see cref="IComposableLut.MaxInputValue"/>.
		/// </summary>
		public override sealed int LastMappedPixelValue
		{
			get { return base.MaxInputValue; }
		}

		/// <summary>
		/// Gets the Lut's data, lazily created.
		/// </summary>
		public override sealed double[] Data
		{
			get
			{
				if (_data == null)
				{
					_data = new double[Length];
					Create();
				}

				return _data;
			}
		}

		/// <summary>
		/// Looks up and returns a value at a particular index in the Lut.
		/// </summary>
		public override sealed double this[int index]
		{
			get { return base[index]; }
			protected set { base[index] = value; }
		}

		double IModalityLut.this[int input]
		{
			get { return this[input]; }
		}

		double IComposableLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
		}

		/// <summary>
		/// Inheritors must implement this method and populate the Lut using an algorithm.
		/// </summary>
		protected abstract void Create();

		/// <summary>
		/// Fires the <see cref="ComposableLut.LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the Lut has changed.
		/// </remarks>
		protected override void OnLutChanged()
		{
			Clear();
		}

		/// <summary>
		/// Clears the data in the Lut; the Lut can be recreated at will by calling <see cref="Create"/>.
		/// </summary>
		public void Clear()
		{
			_data = null;
		}
	}
}