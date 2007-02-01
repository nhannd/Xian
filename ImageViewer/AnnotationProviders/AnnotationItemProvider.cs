using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	public abstract class AnnotationItemProvider : IAnnotationItemProvider
	{
		private string _identifier;
		private string _displayName;
		private IEnumerable<IAnnotationItem> _annotationItems;

		protected AnnotationItemProvider(string identifier)
			: this(identifier, null)
		{
		}

		protected AnnotationItemProvider(string identifier, IAnnotationResourceResolver resolver)
		{
			Platform.CheckForEmptyString(identifier, "identifier");

			if (resolver == null)
				resolver = new AnnotationResourceResolver(this);

			_identifier = identifier;
			_displayName = resolver.ResolveDisplayName(identifier);
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		protected abstract List<IAnnotationItem> AnnotationItems { get; }
		
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
				_annotationItems = this.AnnotationItems.AsReadOnly();

			return _annotationItems;
		}

		#endregion
	}
}
