using System;

namespace ClearCanvas.ImageViewer 
{
	/// <summary>
	/// Defines an interface for image layout management.
	/// </summary>
	/// <remarks>If you want to implement your own hanging protocol engine,
	/// you need to 1) implement this interface and 2) mark your class
	/// with the <code>[ExtensionOf(typeof(LayoutManagerExtensionPoint))]</code>
	/// attribute.</remarks>
	public interface ILayoutManager : IDisposable
	{
		/// <summary>
		/// Lays out images on the specified image viewer.
		/// </summary>
		/// <param name="imageViewer"></param>
		void Layout(IImageViewer imageViewer);
	}
}