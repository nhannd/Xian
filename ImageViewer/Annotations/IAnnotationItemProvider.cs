using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Annotations
{
	public interface IAnnotationItemProvider
	{
		string GetIdentifier();
		string GetDisplayName();
		IEnumerable<IAnnotationItem> GetAnnotationItems();
	}
}
