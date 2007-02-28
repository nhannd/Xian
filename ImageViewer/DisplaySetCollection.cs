using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IDisplaySet"/> objects.
	/// </summary>
	public class DisplaySetCollection : ObservableList<IDisplaySet, DisplaySetEventArgs>
	{
		public DisplaySetCollection()
		{

		}
	}
}
