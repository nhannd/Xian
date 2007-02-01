using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	public abstract class ResourceResolvingAnnotationItem : IAnnotationItem
	{
		private string _identifier;
		private string _displayName;
		private string _label;

#if UNIT_TESTS
		internal readonly bool _allowEmptyLabel;
#endif

		private IAnnotationItemProvider _ownerProvider;

		protected ResourceResolvingAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: this(identifier, ownerProvider, false)
		{ 
		}

		protected ResourceResolvingAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider, bool allowEmptyLabel)
			: this(identifier, ownerProvider, allowEmptyLabel, null)
		{
		}

		protected ResourceResolvingAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider, bool allowEmptyLabel, IAnnotationResourceResolver resolver)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForNullReference(ownerProvider, "ownerProvider");

			_ownerProvider = ownerProvider;
			_identifier = identifier;

			if (resolver == null)
				resolver = new AnnotationResourceResolver(this);

#if UNIT_TESTS
			_allowEmptyLabel = allowEmptyLabel;
#endif

			_displayName = resolver.ResolveDisplayName(identifier);
			_label = resolver.ResolveLabel(identifier, allowEmptyLabel);
		}

		public string Identifier
		{
			get { return _identifier; }
			protected set { _identifier = value; }
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

		public IAnnotationItemProvider OwnerProvider
		{
			get { return _ownerProvider; }
		}

		#region IAnnotationItem

		public string GetIdentifier()
		{
			return this.Identifier;
		}

		public string GetDisplayName()
		{
			return this.DisplayName;
		}

		public string GetLabel()
		{
			return this.Label;
		}

		public abstract string GetAnnotationText(IPresentationImage presentationImage);

		#endregion
	}
}
