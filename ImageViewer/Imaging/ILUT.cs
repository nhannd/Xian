using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface ILUT
	{
		int NumEntries
		{
			get;
		}

		int this[int index]
		{
			get;
			set;
		}
	}
}
