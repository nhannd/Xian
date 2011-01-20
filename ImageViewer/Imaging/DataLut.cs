#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Base implementation for <see cref="IDataLut"/>s.
	/// </summary>
	/// <remarks>
	/// Normally, you should not have to inherit directly from this class.
	/// <see cref="SimpleDataLut"/> or <see cref="GeneratedDataLut"/> should cover
	/// most, if not all, common use cases.
	/// </remarks>
	[Cloneable(true)]
	public abstract class DataLut : ComposableLut, IDataLut
	{
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DataLut()
		{
		}
		
		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (value == _minInputValue)
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (value == _maxInputValue)
					return;

				_maxInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public override int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set
			{
				if (_minOutputValue == value)
					return;

				_minOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public override int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set
			{
				if (value == _maxOutputValue)
					return;

				_maxOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		public override int this[int index]
		{
			get
			{
				if (index <= FirstMappedPixelValue)
					return Data[0];
				else if (index >= LastMappedPixelValue)
					return Data[this.Length - 1];
				else
					return Data[index - this.FirstMappedPixelValue];
			}
			protected set
			{
				if (index < this.FirstMappedPixelValue || index > this.LastMappedPixelValue)
					return;

				this.Data[index - this.FirstMappedPixelValue] = value;
			}
		}

		///<summary>
		/// Gets the length of <see cref="Data"/>.
		///</summary>
		/// <remarks>
		/// The reason for this member's existence is that <see cref="Data"/> may
		/// not yet exist; this value is based solely on <see cref="IDataLut.FirstMappedPixelValue"/>
		/// and <see cref="DataLut.LastMappedPixelValue"/>.
		/// </remarks>
		public int Length
		{
			get
			{
				return 1 + LastMappedPixelValue - FirstMappedPixelValue;
			}
		}

		#region IDataLut Members

		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		public abstract int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the last mapped pixel value.
		/// </summary>
		public abstract int LastMappedPixelValue { get; }

		/// <summary>
		/// Gets the lut data.
		/// </summary>
		public abstract int[] Data { get; }

		#endregion
	}
}
