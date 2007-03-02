using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer 
{
	/// <summary>
	/// Provides an interface for image layout management.
	/// </summary>
	/// <remarks>If you want to implement your own hanging protocol engine,
	/// you need to 1) implement this interface and 2) mark your class
	/// with the <code>[ExtensionOf(typeof(LayoutManagerExtensionPoint))]</code>
	/// attribute.</remarks>
	public interface ILayoutManager : IDisposable
	{
		/// <summary>
		/// Sets the owning <see cref="IImageViewer"/>.
		/// </summary>
		/// <param name="imageViewer">The owning <see cref="IImageViewer"/>.</param>
		void SetImageViewer(IImageViewer imageViewer);

		/// <summary>
		/// Lays out images.
		/// </summary>
		void Layout();
	}
}