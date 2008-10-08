using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Extension point for tools that operate on a <see cref="GalleryComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public class GalleryToolExtensionPoint : ExtensionPoint<ITool> {}

	/// <summary>
	/// <see cref="IToolContext"/> class for tools that operate on a <see cref="GalleryComponent"/>.
	/// </summary>
	public interface IGalleryToolContext : IToolContext
	{
		/// <summary>
		/// Indicates that the current selection of <see cref="IGalleryItem"/>s in the gallery has changed.
		/// </summary>
		event EventHandler SelectionChanged;

		/// <summary>
		/// Indicates that an <see cref="IGalleryItem"/> in the gallery has been activated
		/// </summary>
		event GalleryItemEventHandler ItemActivated;

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> that the <see cref="GalleryComponent"/> is on.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the underlying <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s.
		/// </summary>
		IBindingList DataSource { get; }

		/// <summary>
		/// Gets the current selection of <see cref="IGalleryItem"/>s.
		/// </summary>
		ISelection Selection { get; }

		/// <summary>
		/// Gets the data objects of the current selection of <see cref="IGalleryItem"/>s.
		/// </summary>
		ISelection SelectedData { get; }

		/// <summary>
		/// Activates the specified <see cref="IGalleryItem"/>.
		/// </summary>
		/// <param name="item">The item to activate.</param>
		void Activate(IGalleryItem item);

		/// <summary>
		/// Selects the specified <see cref="IGalleryItem"/>s.
		/// </summary>
		/// <remarks>
		/// Unselection of all items can be accomplished by passing an empty enumeration to <see cref="Select(IEnumerable{IGalleryItem})"/>.
		/// </remarks>
		/// <param name="selection">The items to select.</param>
		void Select(IEnumerable<IGalleryItem> selection);

		/// <summary>
		/// Selects the specified <see cref="IGalleryItem"/>.
		/// </summary>
		/// <remarks>
		/// Unselection of all items can be accomplished by passing an empty enumeration to <see cref="Select(IEnumerable{IGalleryItem})"/>.
		/// </remarks>
		/// <param name="item">The item to select.</param>
		void Select(IGalleryItem item);
	}
}