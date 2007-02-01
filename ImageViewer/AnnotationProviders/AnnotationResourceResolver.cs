using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	public class AnnotationResourceResolver : ResourceResolver, IAnnotationResourceResolver
	{
		protected char replaceChar = '.';
		protected char replaceWithChar = '_';

		public AnnotationResourceResolver(object target)
			: base(new Assembly[] { target.GetType().Assembly, typeof(AnnotationResourceResolver).Assembly })
		{
		}

		public AnnotationResourceResolver(Assembly assembly)
			: base(assembly)
		{
		}

		public AnnotationResourceResolver(Assembly[] assemblies)
			: base(assemblies)
		{
		}

		public string ResolveLabel(string annotationIdentifier)
		{
			return ResolveLabel(annotationIdentifier, false);
		}

		#region IAnnotationResourceResolver Members

		public virtual string ResolveLabel(string annotationIdentifier, bool allowNoResource)
		{
			Platform.CheckForEmptyString(annotationIdentifier, "annotationIdentifier"); 
			
			string resourceString = String.Format("{0}{1}{2}", annotationIdentifier, replaceChar, "Label");
			resourceString = resourceString.Replace(replaceChar, replaceWithChar);

			string label = base.LocalizeString(resourceString);

			if (label == resourceString && !allowNoResource)
			{
#if DEBUG
				throw new NotImplementedException("AnnotationItem has no associated label: " + annotationIdentifier);
#else
				label = SR.Unknown;
#endif
			}

			return label;
		}

		public virtual string ResolveDisplayName(string annotationIdentifier)
		{
			Platform.CheckForEmptyString(annotationIdentifier, "annotationIdentifier"); 
			
			string resourceString = String.Format("{0}{1}{2}", annotationIdentifier, replaceChar, "DisplayName");
			resourceString = resourceString.Replace(replaceChar, replaceWithChar);

			string displayName = base.LocalizeString(resourceString);

			if (displayName == resourceString)
			{
#if DEBUG
				throw new NotImplementedException("AnnotationItem has no display name: " + annotationIdentifier);
#else
				displayName = SR.Unknown;
#endif
			}

			return displayName;
		}

		#endregion
	}
}
