using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public interface IAnnotationLayout
	{
		IEnumerable<AnnotationBox> AnnotationBoxes { get; }
	}
}
