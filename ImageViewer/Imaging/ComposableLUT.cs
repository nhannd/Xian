using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A LUT that can be added to <see cref="LUTCollection"/>.
	/// </summary>
	public class ComposableLUT : LUT, IComposableLUT
	{
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		/// <summary>
		/// Initializes a new instance of a <see cref="ComposableLUT"/> with an
		/// input range specified by the minimum and maximum input values.
		/// </summary>
		/// <param name="minInputValue"></param>
		/// <param name="maxInputValue"></param>
		public ComposableLUT(
			int minInputValue,
			int maxInputValue) : base(maxInputValue - minInputValue + 1)
		{
			_minInputValue = minInputValue;
			_maxInputValue = maxInputValue;
		}

		#region IGrayscaleLUT Members

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		public int MinInputValue
		{
			get { return _minInputValue; }
			protected set { _minInputValue = value; }
		}

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		public int MaxInputValue
		{
			get { return _maxInputValue; }
			protected set { _maxInputValue = value; }
		}

		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		public virtual int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set { _minOutputValue = value; }
		}

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		public virtual int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set { _maxOutputValue = value; }
		}

		#endregion

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, _minInputValue, _maxInputValue, this);
				return base.Table[index - _minInputValue];
			}
			set
			{
				Platform.CheckIndexRange(index, _minInputValue, _maxInputValue, this);
				base.Table[index - _minInputValue] = value;
			}
		}
	}
}
