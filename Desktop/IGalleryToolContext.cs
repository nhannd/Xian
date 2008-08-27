using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	[ExtensionPoint]
	public class GalleryToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IGalleryToolContext : IToolContext
	{
		event EventHandler SelectionChanged;
		event GalleryItemEventHandler ItemActivated;
		IDesktopWindow DesktopWindow { get; }
		IBindingList DataSource { get; }
		ISelection Selection { get; }
		ISelection SelectedData { get; }
		void Activate(IGalleryItem item);
		void Select(IEnumerable<IGalleryItem> selection);
		void Select(IGalleryItem item);
	}
}