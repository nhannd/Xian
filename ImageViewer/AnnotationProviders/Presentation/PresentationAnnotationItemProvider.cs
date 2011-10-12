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
//using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PresentationAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;
		
		public PresentationAnnotationItemProvider()
			: base("AnnotationItemProviders.Presentation", new AnnotationResourceResolver(typeof(PresentationAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>
			                       {
			                           new ZoomAnnotationItem(),
			                           new AppliedLutAnnotationItem(),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Left),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Top),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Right),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Bottom),
			                           new DFOVAnnotationItem(),
			                           new ImageCalibrationAnnotationItem(),
			                           new DisplaySetDescriptionAnnotationItem(),
			                           new DisplaySetNumberAnnotationItem()
			                       };
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
