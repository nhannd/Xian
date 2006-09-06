using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public interface IAnnotationItem
	{
		string GetIdentifier();
		string GetDisplayName();
		string GetLabel();
		string GetAnnotationText(PresentationImage presentationImage);
	}
}
