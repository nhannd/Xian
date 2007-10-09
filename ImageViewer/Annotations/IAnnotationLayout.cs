using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Annotations
{
	public interface IAnnotationLayout
	{
		IEnumerable<AnnotationBox> AnnotationBoxes { get; }
	}
}
