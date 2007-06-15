using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A collection of <see cref="IComposableLUT"/> objects.
	/// </summary>
	public class LUTCollection : ObservableList<IComposableLUT, LUTEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="LUTCollection"/>.
		/// </summary>
		public LUTCollection()
		{
		}
	}
}
