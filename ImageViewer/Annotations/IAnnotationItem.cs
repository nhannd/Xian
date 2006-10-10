using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public interface IAnnotationItem
	{
		string GetIdentifier();
		string GetDisplayName();
		string GetLabel();
		string GetAnnotationText(IPresentationImage presentationImage);
	}
}
