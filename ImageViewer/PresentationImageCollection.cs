using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IPresentationImage"/> objects.
	/// </summary>
	public class PresentationImageCollection : ObservableList<IPresentationImage, PresentationImageEventArgs>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="PresentationImageCollection"/>.
		/// </summary>
		public PresentationImageCollection()
		{

		}
	}
}
