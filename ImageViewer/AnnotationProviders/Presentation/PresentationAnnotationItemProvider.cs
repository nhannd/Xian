#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PresentationAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;
		
		public PresentationAnnotationItemProvider()
			: base("AnnotationItemProviders.Presentation", new AnnotationResourceResolver(typeof(PresentationAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			_annotationItems.Add((IAnnotationItem)new ZoomAnnotationItem());
			_annotationItems.Add((IAnnotationItem)new AppliedLutAnnotationItem());

			_annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Left));
			_annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Top));
			_annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Right));
			_annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Bottom));

			_annotationItems.Add(new DFOVAnnotationItem());
			_annotationItems.Add(new DisplaySetDescriptionAnnotationItem());
			_annotationItems.Add(new DisplaySetNumberAnnotationItem());
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
