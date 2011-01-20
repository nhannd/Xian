#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;

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
