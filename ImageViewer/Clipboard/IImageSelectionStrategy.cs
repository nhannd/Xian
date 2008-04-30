using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Clipboard
{
	public interface IImageSelectionStrategy
	{
		string Description { get; }
		IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet);
	}
}
