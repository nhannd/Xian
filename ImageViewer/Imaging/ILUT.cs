using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a lookup table.
	/// </summary>
	public interface ILut
	{
		/// <summary>
		/// Gets the number of entries in the LUT.
		/// </summary>
		int Length
		{
			get;
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		int this[int index]
		{
			get;
			set;
		}
	}
}
