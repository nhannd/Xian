using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	public abstract class AnnotationItem : IAnnotationItem
	{
		private string _identifier;
		private string _displayName;
		private string _label;

		protected AnnotationItem(string identifier, string displayName, string label)
		{
			Platform.CheckForEmptyString(identifier, "identifier"); 
			
			_identifier = identifier;
			_displayName = displayName;
			_label = label;
		}

		protected AnnotationItem()
		{ 
		}

		public string Identifier
		{
			get { return _identifier; }
			protected set
			{
				Platform.CheckForEmptyString(value, "value"); 
				_identifier = value;
			}
		}

		public string DisplayName
		{
			get { return _displayName; }
			protected set { _displayName = value; }
		}

		public string Label
		{
			get { return _label; }
			protected set { _label = value; }
		}

		#region IAnnotationItem Members

		public string GetIdentifier()
		{
			return _identifier;
		}

		public string GetDisplayName()
		{
			return _displayName;
		}

		public string GetLabel()
		{
			return _label;
		}

		public abstract string GetAnnotationText(IPresentationImage presentationImage);

		#endregion
	}
}
