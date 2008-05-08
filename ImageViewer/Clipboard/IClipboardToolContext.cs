using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// The clipboard tool context.
	/// </summary>
	/// <remarks>
	/// Provides clipboard tools access to items in the clipboard.
	/// </remarks>
	public interface IClipboardToolContext : IToolContext
	{
		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> in which the clipboard is hosted.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets a list of the items in the clipboard.
		/// </summary>
		IList<IClipboardItem> ClipboardItems { get; }

		/// <summary>
		/// Gets a list of the selected items in the clipboard.
		/// </summary>
		ReadOnlyCollection<IClipboardItem> SelectedClipboardItems { get; }

		/// <summary>
		/// Occurs when the list of clipboard items has changed.
		/// </summary>
		event EventHandler ClipboardItemsChanged;

		/// <summary>
		/// Occurs when the list of selected clipboard items has changed.
		/// </summary>
		event EventHandler SelectedClipboardItemsChanged;
	}
}
