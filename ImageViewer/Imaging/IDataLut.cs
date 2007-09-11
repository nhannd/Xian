using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IDataLut : ILut
	{
		/// <summary>
		/// Gets the length of the data lut.
		/// </summary>
		uint Length { get; }
	}
}
