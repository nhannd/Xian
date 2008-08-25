using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	[ExtensionPoint]
	public class GalleryToolExtensionPoint : ExtensionPoint<IGalleryToolContext>
	{
	}

	public interface IGalleryToolContext : IToolContext
	{
		IBindingList DataSource { get; }
		ISelection Selection { get; }
		ISelection SelectedData { get; }
		void Select(IEnumerable<IGalleryItem> selection);
		void Select(IGalleryItem item);
	}
}