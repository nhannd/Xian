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
	/// An implementation of a <see cref="IDataModalityLut"/> whose size and data do not change.
	/// </summary>
	[Cloneable]
	public class SimpleDataModalityLut : DataModalityLut, IModalityLut
	{
		[CloneIgnore]
		private readonly double[] _data;

		private readonly string _key;
		private readonly string _description;
		private readonly int _firstMappedPixelValue;

		/// <summary>
		/// Initializes a new instance of <see cref="SimpleDataModalityLut"/>.
		/// </summary>
		/// <param name="firstMappedPixelValue">The value of the first stored pixel value mapped by <paramref name="data"/>.</param>
		/// <param name="data">The data table mapping stored pixel values to manfuacturer-independent values.</param>
		/// <param name="minOutputValue">The minimum output value of the lookup table.</param>
		/// <param name="maxOutputValue">The maximum output value of the lookup table.</param>
		/// <param name="key">A key string suitable for identifying the characteristics of the lookup table.</param>
		/// <param name="description">A description of the characteristics of the lookup table.</param>
		public SimpleDataModalityLut(int firstMappedPixelValue, double[] data, double minOutputValue, double maxOutputValue, string key, string description)
		{
			Platform.CheckForNullReference(data, "data");
			Platform.CheckForEmptyString(key, "key");
			Platform.CheckForEmptyString(description, "description");

			_data = data;

			_key = key;
			_description = description;

			_firstMappedPixelValue = firstMappedPixelValue;

			base.MinInputValue = _firstMappedPixelValue;
			base.MaxInputValue = LastMappedPixelValue;
			base.MinOutputValue = minOutputValue;
			base.MaxOutputValue = maxOutputValue;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected SimpleDataModalityLut(SimpleDataModalityLut source, ICloningContext context)
		{
			context.CloneFields(source, this);

			//clone the actual buffer
			_data = (double[]) source._data.Clone();
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override double MinOutputValue
		{
			get { return base.MinOutputValue; }
			protected set { }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override double MaxOutputValue
		{
			get { return base.MaxOutputValue; }
			protected set { }
		}

		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		public override sealed int FirstMappedPixelValue
		{
			get { return _firstMappedPixelValue; }
		}

		/// <summary>
		/// Gets the last mapped pixel value.
		/// </summary>
		public override sealed int LastMappedPixelValue
		{
			get { return _firstMappedPixelValue + _data.Length - 1; }
		}

		/// <summary>
		/// Gets the lookup table data.
		/// </summary>
		public override double[] Data
		{
			get { return _data; }
		}

		double IModalityLut.this[int input]
		{
			get { return this[input]; }
		}

		double IComposableLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
		}

		public override string GetKey()
		{
			return _key;
		}

		public override string GetDescription()
		{
			return _description;
		}
	}
}