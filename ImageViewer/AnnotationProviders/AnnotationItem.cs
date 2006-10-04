using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	public abstract class AnnotationItem : IAnnotationItem
	{
		private string _identifier;
		private string _displayName;
		private string _label;

		private IAnnotationItemProvider _ownerProvider;

		protected AnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
		{
			_ownerProvider = ownerProvider;

			_identifier = identifier;

			string resourceString = _identifier.Replace('.', '_');

			_displayName = SR.ResourceManager.GetString(resourceString + "_DisplayName");
			
			if (string.IsNullOrEmpty(_displayName))
			{
#if DEBUG
				throw new NotImplementedException("AnnotationItem has no display name: " + this.GetType().ToString());
#else
				_displayName = SR.Unknown;
#endif
			}


			_label = SR.ResourceManager.GetString(resourceString + "_Label");
			if (string.IsNullOrEmpty(_label))
			{
#if DEBUG
				throw new NotImplementedException("AnnotationItem has no associated label: " + this.GetType().ToString());
#else
				_label = SR.Unknown;
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

		public string Label
		{
			get { return _label; }
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

		public abstract string GetAnnotationText(PresentationImage presentationImage);

		#endregion
	}
}
