using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A lookup table.
	/// </summary>
	public class Lut : ILut
	{
		private int _length;
		private int[] _table;

		protected Lut()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="LUT"/> with the specified
		/// number of entries.
		/// </summary>
		/// <param name="length"></param>
		public Lut(int length)
		{
			Platform.CheckPositive(length, "length");
			_length = length;
		}

		/// <summary>
		/// Returns the number of entries in the LUT.
		/// </summary>
		public int Length
		{
			get { return _length; }
			protected set { _length = value; }
		}

		/// <summary>
		/// Gets the array in which LUT values are stored.
		/// </summary>
		protected int[] Table
		{
			get
			{
				if (_table == null)
					_table = new int[_length];

				return _table;
			}
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, _length - 1, this);
				return this.Table[index];
			}
			set
			{
				Platform.CheckIndexRange(index, 0, _length - 1, this);
				this.Table[index] = value;
			}
		}
	}
}
