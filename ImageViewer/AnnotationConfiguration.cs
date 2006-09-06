using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer
{
	public abstract class AnnotationConfiguration
	{
		private string _identifier;
		private string _displayName;
		private AnnotationLayout _annotationLayout;

		public AnnotationConfiguration
			(
				string identifier,
				string displayName,
				AnnotationLayout annotationLayout
			)
		{
			_identifier = identifier;
			_displayName = displayName;
			_annotationLayout = annotationLayout;
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		protected abstract bool MeetsCriteria(PresentationImage presentationImage);

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get { return _annotationLayout.AnnotationBoxes; }
		}

		public IEnumerable<AnnotationBox> GetAnnotationBoxes(PresentationImage presentationImage)
		{
			if (!MeetsCriteria(presentationImage))
				return null;

			return _annotationLayout.AnnotationBoxes;
		}
	}
}
