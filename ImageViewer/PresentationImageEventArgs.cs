using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="PresentationImageCollection"/> events.
	/// </summary>
	public class PresentationImageEventArgs : CollectionEventArgs<IPresentationImage>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="PresentationImageEventArgs"/>
		/// </summary>
		public PresentationImageEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageEventArgs"/> with
		/// a specified <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="presentationImage"></param>
		public PresentationImageEventArgs(IPresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");

			base.Item = presentationImage;
		}

		/// <summary>
		/// Gets the <see cref="IPresentationImage"/>.
		/// </summary>
		public IPresentationImage PresentationImage { get { return base.Item; } }
	}
}
