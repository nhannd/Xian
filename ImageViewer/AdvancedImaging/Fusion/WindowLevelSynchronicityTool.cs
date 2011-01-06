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

			base.ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;
			base.ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			base.ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;

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

					// find any available display set containing the same series as the individual layers and replicate its VoiLutManager memento
					object baseMemento = null, overlayMemento = null;
					var descriptor = (PETFusionDisplaySetDescriptor) e.NewDisplaySet.Descriptor;
					foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
					{
						var selectedImage = imageBox.TopLeftPresentationImage;
						if (imageBox.DisplaySet == null || imageBox.DisplaySet.Descriptor is PETFusionDisplaySetDescriptor
						    || !(selectedImage is IImageSopProvider && selectedImage is IVoiLutProvider))
							continue;

						var seriesUid = ((IImageSopProvider) selectedImage).ImageSop.SeriesInstanceUid;
						if (baseMemento == null && seriesUid == descriptor.SourceSeries.SeriesInstanceUid)
							baseMemento = ((IVoiLutProvider) imageBox.TopLeftPresentationImage).VoiLutManager.CreateMemento();
						else if (overlayMemento == null && seriesUid == descriptor.PETSeries.SeriesInstanceUid)
							overlayMemento = ((IVoiLutProvider) imageBox.TopLeftPresentationImage).VoiLutManager.CreateMemento();

						if (baseMemento != null && overlayMemento != null)
							break;
					}

					if (baseMemento == null || overlayMemento == null)
					{
						var fusionImage = (FusionPresentationImage) e.NewDisplaySet.PresentationImages[0];
						if (baseMemento == null)
							baseMemento = GetInitialVoiLutMemento(fusionImage.Frame);
						if (overlayMemento == null)
							overlayMemento = GetInitialVoiLutMemento(fusionImage.OverlayFrameData.OverlayData.Frames[0]);
					}

					foreach (FusionPresentationImage image in e.NewDisplaySet.PresentationImages)
					{
						if (baseMemento != null)
							image.SetBaseVoiLutManagerMemento(baseMemento);
						if (overlayMemento != null)
							image.SetOverlayVoiLutManagerMemento(overlayMemento);
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
					object memento = ((IVoiLutProvider) e.PresentationImage).VoiLutManager.CreateMemento();
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
								var changed = image.SetBaseVoiLutManagerMemento(memento);
								anyVisibleChange |= (image.Visible && changed);
							}
						}
						else if (descriptor.PETSeries.SeriesInstanceUid == seriesInstanceUid)
						{
							foreach (FusionPresentationImage image in displaySet.PresentationImages)
							{
								// written this way because we want to set the memento regardless whether or not the image is visible
								var changed = image.SetOverlayVoiLutManagerMemento(memento);
								anyVisibleChange |= (image.Visible && changed);
							}
						}

						if (anyVisibleChange)
							displaySet.Draw();
					}
				}
			}
		}

		private static object GetInitialVoiLutMemento(Frame frame)
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
					var voiLutManager = (IVoiLutManager) new VoiLutManagerProxy();
					if (voiLut != null)
						voiLutManager.InstallVoiLut(voiLut);
					return voiLutManager.CreateMemento();
				}
			}
			return null;
		}
	}
}