using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.TextOverlay
{
	public abstract class AnnotationItemProvider : IAnnotationItemProvider
	{
		private string _identifier;
		private string _displayName;
		private IEnumerable<IAnnotationItem> _annotationItems;

		protected AnnotationItemProvider(string identifier)
		{
			_identifier = identifier;

			string resourceString = _identifier.Replace('.', '_');

			_displayName = SR.ResourceManager.GetString(resourceString + "_DisplayName");
			if (string.IsNullOrEmpty(_displayName))
			{
#if DEBUG
				throw new NotImplementedException("AnnotationItemProvider has no display name: " + this.GetType().ToString());
#else
				_displayName = SR.Unknown;
#endif
			}
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		#region IAnnotationItemProvider Members

		public string GetIdentifier()
		{
			return this.Identifier;
		}

		public string GetDisplayName()
		{
			return this.DisplayName;
		}

		public IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			if (_annotationItems == null)
			{
				_annotationItems = this.AnnotationItems.AsReadOnly();
			}

			return _annotationItems;
		}

		protected abstract List<IAnnotationItem> AnnotationItems { get; }

		#endregion
	}
}
