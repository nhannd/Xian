using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLut : IComposableLut
	{
		string Name { get; }
	}
}
