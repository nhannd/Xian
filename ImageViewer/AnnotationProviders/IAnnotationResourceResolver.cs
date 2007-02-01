using System;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	public interface IAnnotationResourceResolver
	{
		string ResolveDisplayName(string annotationIdentifier);
		string ResolveLabel(string annotationIdentifier, bool allowNoResource);
	}
}
