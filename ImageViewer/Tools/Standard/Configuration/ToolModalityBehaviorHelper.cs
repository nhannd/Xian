#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.Configuration
{
	internal sealed class ToolModalityBehaviorHelper
	{
		private readonly IImageViewer _imageViewer;

		public ToolModalityBehaviorHelper(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
		}

		public ToolModalityBehavior Behavior
		{
			get { return ToolSettings.Default.CachedToolModalityBehavior.GetEntryOrDefault(SelectedModality); }
		}

		private string SelectedModality
		{
			get
			{
				var selectedImage = _imageViewer.SelectedPresentationImage;
				if (selectedImage == null)
					return string.Empty;

				if (selectedImage.ParentDisplaySet != null)
				{
					var dicomDescriptor = selectedImage.ParentDisplaySet.Descriptor as IDicomDisplaySetDescriptor;
					if (dicomDescriptor != null && dicomDescriptor.SourceSeries != null)
					{
						var series = _imageViewer.StudyTree.GetSeries(dicomDescriptor.SourceSeries.SeriesInstanceUid ?? string.Empty);
						if (series != null && series.Sops.Count > 0 && series.Sops[0].SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
							return @"KO";
					}
				}

				var imageSopProvider = selectedImage as IImageSopProvider;
				return imageSopProvider != null ? imageSopProvider.ImageSop.Modality : string.Empty;
			}
		}
	}
}