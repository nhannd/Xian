using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A calculated LUT.
	/// </summary>
	/// <remarks>
	/// A <see cref="CalculatedLUT"/> stores no actual values, but instead
	/// calculates the value whenever a lookup is performed.
	/// </remarks>
	public abstract class CalculatedLUT : IComposableLUT
	{
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		#region ILUT Members

		/// <summary>
		/// Gets the number of entries in the LUT.
		/// </summary>
		public int Length
		{
			get	
			{ 
				return _maxInputValue - _minInputValue + 1; 
			}
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public abstract int this[int index]
		{
			get;
			set;
		}

		#endregion

		#region IGrayscaleLUT Members

		/// <summary>
		/// Gets or sets the minimum allowable input value of the LUT.
		/// </summary>
		public int MinInputValue
		{
			get { return _minInputValue; }
			protected set { _minInputValue = value; }
		}

		/// <summary>
		/// Gets or sets the maximum allowable input value of the LUT.
		/// </summary>
		public int MaxInputValue
		{
			get	{ return _maxInputValue; }
			protected set { _maxInputValue = value; }
		}

		/// <summary>
		/// Gets or sets the minimum allowable output value of the LUT.
		/// </summary>
		public int MinOutputValue
		{
			get	{ return _minOutputValue; }
			protected set { _minOutputValue = value; }
		}

		/// <summary>
		/// Gets or sets the maximum allowable output value of the LUT.
		/// </summary>
		public int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set { _maxOutputValue = value; }
		}

		#endregion
	}
}
