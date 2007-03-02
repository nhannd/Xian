using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IImageSet"/> objects.
	/// </summary>
	public class ImageSetCollection : ObservableList<IImageSet, ImageSetEventArgs>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="ImageSetCollection"/>.
		/// </summary>
		public ImageSetCollection()
		{

		}
	}
}
