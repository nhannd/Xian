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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class WindowLevelSynchronicityTool : ImageViewerTool
	{
		private readonly IList<IDisplaySet> _fusionDisplaySets = new List<IDisplaySet>();

		public override void Initialize()
		{
			base.Initialize();

			ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;
			ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
		}

		protected override void Dispose(bool disposing)
		{
			ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;

			base.Dispose(disposing);
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (e.OldDisplaySet != null)
			{
				_fusionDisplaySets.Remove(e.OldDisplaySet);
			}

			if (e.NewDisplaySet != null && e.NewDisplaySet.Descriptor is PETFusionDisplaySetDescriptor)
			{
				if (e.NewDisplaySet.ImageBox != null)
				{
					_fusionDisplaySets.Add(e.NewDisplaySet);

					// no point doing all this to find an appropriate VOI LUT if there are no images in the display set - but do update the fusionDisplaySets list!
					if (e.NewDisplaySet.PresentationImages.Count == 0)
						return;

					// find any available display set containing the same series as the individual layers and replicate its VOI LUT
					IComposableLut baseVoiLut = null, overlayVoiLut = null;
					var descriptor = (PETFusionDisplaySetDescriptor) e.NewDisplaySet.Descriptor;
					foreach (IImageBox imageBox in ImageViewer.PhysicalWorkspace.ImageBoxes)
					{
						var selectedImage = imageBox.TopLeftPresentationImage;
						if (imageBox.DisplaySet == null || imageBox.DisplaySet.Descriptor is PETFusionDisplaySetDescriptor
						    || !(selectedImage is IImageSopProvider && selectedImage is IVoiLutProvider))
							continue;

						var seriesUid = ((IImageSopProvider) selectedImage).ImageSop.SeriesInstanceUid;
						if (baseVoiLut == null && seriesUid == descriptor.SourceSeries.SeriesInstanceUid)
							baseVoiLut = ((IVoiLutProvider) imageBox.TopLeftPresentationImage).VoiLutManager.VoiLut.Clone();
						else if (overlayVoiLut == null && seriesUid == descriptor.PETSeries.SeriesInstanceUid)
							overlayVoiLut = ((IVoiLutProvider) imageBox.TopLeftPresentationImage).VoiLutManager.VoiLut.Clone();

						if (baseVoiLut != null && overlayVoiLut != null)
							break;
					}

					if (baseVoiLut == null || overlayVoiLut == null)
					{
						var fusionImage = (FusionPresentationImage) e.NewDisplaySet.PresentationImages[0];
						if (baseVoiLut == null)
							baseVoiLut = GetInitialVoiLutMemento(fusionImage.Frame);
						if (overlayVoiLut == null)
							overlayVoiLut = GetInitialVoiLutMemento(fusionImage.OverlayFrameData.OverlayData.Frames[0]);
					}

					foreach (FusionPresentationImage image in e.NewDisplaySet.PresentationImages)
					{
						if (baseVoiLut != null)
							image.BaseVoiLutManager.InstallVoiLut(baseVoiLut);
						if (overlayVoiLut != null)
							image.OverlayVoiLutManager.InstallVoiLut(overlayVoiLut);
					}
				}
			}
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			if (_fusionDisplaySets.Count == 0)
				return;

			if (!(e.PresentationImage is FusionPresentationImage))
			{
				if (e.PresentationImage is IImageSopProvider && e.PresentationImage is IVoiLutProvider)
				{
					IComposableLut voiLut = ((IVoiLutProvider) e.PresentationImage).VoiLutManager.VoiLut.Clone();
					string seriesInstanceUid = ((IImageSopProvider) e.PresentationImage).ImageSop.SeriesInstanceUid;

					// find any available display set containing the same series as the individual layers and replicate its VoiLutManager memento
					foreach (IDisplaySet displaySet in _fusionDisplaySets)
					{
						var anyVisibleChange = false;

						var descriptor = (PETFusionDisplaySetDescriptor) displaySet.Descriptor;
						if (descriptor.SourceSeries.SeriesInstanceUid == seriesInstanceUid)
						{
							foreach (FusionPresentationImage image in displaySet.PresentationImages)
							{
								// written this way because we want to set the memento regardless whether or not the image is visible
								image.BaseVoiLutManager.InstallVoiLut(voiLut);
								anyVisibleChange |= (image.Visible);
							}
						}
						else if (descriptor.PETSeries.SeriesInstanceUid == seriesInstanceUid)
						{
							foreach (FusionPresentationImage image in displaySet.PresentationImages)
							{
								// written this way because we want to set the memento regardless whether or not the image is visible
								image.OverlayVoiLutManager.InstallVoiLut(voiLut);
								anyVisibleChange |= (image.Visible);
							}
						}

						if (anyVisibleChange)
							displaySet.Draw();
					}
				}
			}
		}

		private static IComposableLut GetInitialVoiLutMemento(Frame frame)
		{
			if (frame != null)
			{
				using (var image = PresentationImageFactory.Create(frame))
				{
					var voiLut = InitialVoiLutProvider.Instance.GetLut(image);
					if (voiLut == null && image is IImageGraphicProvider)
					{
						//TODO (CR Sept 2010): will this always happen for the PT layer?  Could it be an
						//average of the w/l of the source PT image frames?
						var pixelData = ((IImageGraphicProvider) image).ImageGraphic.PixelData;
						if (pixelData is GrayscalePixelData)
							voiLut = new MinMaxPixelCalculatedLinearLut((GrayscalePixelData) pixelData);
					}
					return voiLut;
				}
			}
			return null;
		}
	}
}