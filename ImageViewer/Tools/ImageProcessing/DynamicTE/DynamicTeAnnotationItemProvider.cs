using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.AnnotationProviders;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class DynamicTeAnnotationItemProvider : IAnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public DynamicTeAnnotationItemProvider()
		{
		}


		#region IAnnotationItemProvider Members

		public string GetIdentifier()
		{
			return "AnnotationItemProviders.Presentation";
		}

		public string GetDisplayName()
		{
			return "Presentation";
		}

		public IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			if (_annotationItems == null)
			{
				_annotationItems = new List<IAnnotationItem>();
				_annotationItems.Add(new DynamicTeAnnotationItem(this));
			}

			return _annotationItems;
		}

		#endregion
	}
}
