using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class PresentationImageCollection : ObservableList<IPresentationImage, PresentationImageEventArgs>
	{
		public PresentationImageCollection()
		{

		}
	}
}
