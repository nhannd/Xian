using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLutManager : IMemorable
	{
		void SetPresentationLut(string name);
		IPresentationLut PresentationLut { get; }
	}
}
