using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a linear VOI LUT.
	/// </summary>
	public interface IVOILUTLinear : IMemorable
	{
		/// <summary>
		/// Gets or sets the window width.
		/// </summary>
		double WindowWidth { get; set; }

		/// <summary>
		/// Gets or sets the window center.
		/// </summary>
		double WindowCenter { get; set; }
	}
}
