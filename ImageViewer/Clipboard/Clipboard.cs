using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// A global image clipboard.
	/// </summary>
	/// <remarks>
	/// The clipboard can be thought of as a "holding area" for images the user has deemed to
	/// be of interest. Clipboard tools can then operate on those images.
	/// </remarks>
	public static class Clipboard
	{
		/// <summary>
		/// Adds an <see cref="IPresentationImage"/> to the clipboard.
		/// </summary>
		/// <param name="image"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IPresentationImage image)
		{
			ClipboardComponent.AddToClipboard(image);
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IDisplaySet"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IDisplaySet"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet)
		{
			ClipboardComponent.AddToClipboard(displaySet);
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="selectionStrategy"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/>s
		/// (as determined by the <paramref name="selectionStrategy"/>) is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet, IImageSelectionStrategy selectionStrategy)
		{
			ClipboardComponent.AddToClipboard(displaySet, selectionStrategy);
		}
	}
}
