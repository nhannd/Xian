using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class ImageBoxCollection : ObservableList<ImageBox, ImageBoxEventArgs>
	{
		public ImageBoxCollection()
		{

		}

		/// <summary>
		/// Creates a copy of the object.
		/// </summary>
		/// <param name="collection"></param>
		/// <remarks>
		/// Creates a <i>shallow</i> copy.  That is, only references to objects
		/// in the collection are copied.
		/// </remarks>
		public ImageBoxCollection(ObservableList<ImageBox, ImageBoxEventArgs> collection) 
			: base(collection)
		{
		}
	}
}
