using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public interface IAnnotationItemProvider
	{
		string GetIdentifier();
		string GetDisplayName();
		IEnumerable<IAnnotationItem> GetAnnotationItems();
	}
}
