using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="ImageBoxCollection"/> events.
	/// </summary>
	public class ImageBoxEventArgs : CollectionEventArgs<IImageBox>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageBoxEventArgs"/>.
		/// </summary>
		public ImageBoxEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageBoxEventArgs"/> with
		/// a specified <see cref="IImageBox"/>
		/// </summary>
		/// <param name="imageBox"></param>
		public ImageBoxEventArgs(IImageBox imageBox)
		{
			Platform.CheckForNullReference(imageBox, "imageBox");

			base.Item = imageBox;
		}

		/// <summary>
		/// Gets the <see cref="IImageBox"/>.
		/// </summary>
		public IImageBox ImageBox { get { return base.Item; } }
	}
}
