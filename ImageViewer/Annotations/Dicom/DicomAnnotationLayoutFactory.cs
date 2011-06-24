#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	internal static class DicomAnnotationLayoutFactory
	{
		public static IAnnotationLayout CreateLayout(IImageSopProvider dicomImage)
		{
			string layoutId = DicomFilteredAnnotationLayoutStore.Instance.GetMatchingStoredLayoutId(dicomImage);
			return AnnotationLayoutFactory.CreateLayout(layoutId);
		}

		public static IAnnotationLayout CreateLayout(List<KeyValuePair<string, string>> filterCandidates)
		{
			string layoutId = DicomFilteredAnnotationLayoutStore.Instance.GetMatchingStoredLayoutId(filterCandidates);
			return AnnotationLayoutFactory.CreateLayout(layoutId);
		}
	}
}