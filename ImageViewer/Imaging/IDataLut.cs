using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Data Lut.
	/// </summary>
	public interface IDataLut : IComposableLut
	{
		/// <summary>
		/// Gets the length of the data lut.
		/// </summary>
		uint Length { get; }
	}
}
