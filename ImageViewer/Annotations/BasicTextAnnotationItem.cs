namespace ClearCanvas.ImageViewer.Annotations
{
	public class BasicTextAnnotationItem : AnnotationItem
	{
		private string _annotationText;

		public BasicTextAnnotationItem(string identifier, string displayName, string label, string annotationText)
			: base(identifier, displayName, label)
		{
			_annotationText = annotationText;
		}

		public string AnnotationText
		{
			get { return _annotationText; }
			set { _annotationText = value; }
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			return _annotationText;
		}
	}
}
