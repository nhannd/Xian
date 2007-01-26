using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public interface IAnnotationLayoutProvider
	{
		IAnnotationLayout AnnotationLayout { get; }
	}
}
