using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Annotations
{
	public sealed class StoredAnnotationBoxGroup
	{
		private string _identifier;
		private AnnotationBox _defaultBoxSettings;
		private List<AnnotationBox> _annotationBoxes;

		public StoredAnnotationBoxGroup(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");

			_identifier = identifier;
			_defaultBoxSettings = new AnnotationBox();
			_annotationBoxes = new List<AnnotationBox>();
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public AnnotationBox DefaultBoxSettings
		{
			get { return _defaultBoxSettings; }
		}

		public AnnotationBox GetNewBox()
		{
			return _defaultBoxSettings.Clone();
		}

		public IList<AnnotationBox> AnnotationBoxes
		{
			get
			{
				return _annotationBoxes;
			}
		}
	}
}