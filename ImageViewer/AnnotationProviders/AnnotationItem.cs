using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	internal abstract class AnnotationItem : ClearCanvas.ImageViewer.Annotations.AnnotationItem
	{
		private IAnnotationItemProvider _ownerProvider;

		protected AnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: this(identifier, ownerProvider, false)
		{ 
		}

		protected AnnotationItem(string identifier, IAnnotationItemProvider ownerProvider, bool allowEmptyLabel)
		{
			_ownerProvider = ownerProvider;

			this.Identifier = identifier;

			string resourceString = this.Identifier.Replace('.', '_');

			this.DisplayName = SR.ResourceManager.GetString(resourceString + "_DisplayName");

			if (string.IsNullOrEmpty(this.DisplayName))
			{
#if DEBUG
				throw new NotImplementedException("AnnotationItem has no display name: " + this.GetType().ToString());
#else
				this.DisplayName = SR.Unknown;
#endif
			}

			this.Label = SR.ResourceManager.GetString(resourceString + "_Label");
			if (string.IsNullOrEmpty(this.Label) && !allowEmptyLabel)
			{
#if DEBUG
					throw new NotImplementedException("AnnotationItem has no associated label: " + this.GetType().ToString());
#else
					this.Label = SR.Unknown;
#endif
				}
		}

		public IAnnotationItemProvider OwnerProvider
		{
			get { return _ownerProvider; }
		}
	}
}
