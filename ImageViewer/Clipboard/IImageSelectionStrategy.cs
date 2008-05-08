using System.Collections.Generic;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// Defines a strategy for image selection.
	/// </summary>
	/// <remarks>
	/// This interface can be used to implement different strategies for
	/// what subset of images in a display set should be sent to the clipboard.
	/// For example, you might have a "ImageRangeStrategy", which would accept
	/// a beginning and ending image.  When the clipboard framework calls
	/// <see cref="GetImages"/>, that range of images is returned.
	/// </remarks>
	public interface IImageSelectionStrategy
	{
		/// <summary>
		/// Gets a description of the image selection strategy.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the subset of <see cref="IPresentationImage"/>s from the specified
		/// <see cref="IDisplaySet"/> that will be sent to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <returns></returns>
		IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet);
	}
}
