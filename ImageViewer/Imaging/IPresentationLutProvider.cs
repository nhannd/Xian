using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLutProvider
	{
		IPresentationLutManager PresentationLutManager { get; }
	}

}
