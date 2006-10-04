using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IGrayscaleLUT : ILUT
	{
		int MinInputValue
		{
			get;
		}

		int MaxInputValue
		{
			get;
		}

		int MinOutputValue
		{
			get;
		}

		int MaxOutputValue
		{
			get;
		}
	}
}
