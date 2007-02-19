using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IComposableLUT : ILUT
	{
		int MinInputValue { get; }

		int MaxInputValue { get; }

		int MinOutputValue { get; }

		int MaxOutputValue { get; }
	}
}
