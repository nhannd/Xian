using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLutProvider
	{
		IVoiLutManager VoiLutManager { get; }
	}
}
