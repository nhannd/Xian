using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public class AnnotationLayout
	{
		private string _identifier;
		private IEnumerable<AnnotationBox> _annotationBoxCollection;

		public string Identifier
		{
			get { return _identifier; }
			set { _identifier = value; }
		}

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get { return _annotationBoxCollection; }
			set { _annotationBoxCollection = value; }
		}
	}
}
