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

					// find any available display set containing the same series as the individual layers and capture its VOI LUT
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
							baseVoiLut = ((IVoiLutProvider) selectedImage).VoiLutManager.VoiLut;
						else if (overlayVoiLut == null && seriesUid == descriptor.PETSeries.SeriesInstanceUid)
							overlayVoiLut = ((IVoiLutProvider) selectedImage).VoiLutManager.VoiLut;

						if (baseVoiLut != null && overlayVoiLut != null)
							break;
					}

					if (baseVoiLut == null || overlayVoiLut == null)
					{
						var fusionImage = (FusionPresentationImage) e.NewDisplaySet.PresentationImages[0];
						if (baseVoiLut == null)
							baseVoiLut = GetInitialVoiLut(fusionImage.Frame);
						if (overlayVoiLut == null)
							overlayVoiLut = GetInitialVoiLut(fusionImage.OverlayFrameData.OverlayData.Frames[0]);
					}

					// replicate the captured VOI LUTs to the fusion images
					foreach (FusionPresentationImage image in e.NewDisplaySet.PresentationImages)
					{
						if (baseVoiLut != null)
							image.BaseVoiLutManager.InstallVoiLut(ReplicateVoiLut(baseVoiLut));
						if (overlayVoiLut != null)
							image.OverlayVoiLutManager.InstallVoiLut(ReplicateVoiLut(overlayVoiLut));
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
					// only synchronize the VOI LUTs if the source LUT is linear - otherwise, leave it alone
					var sourceVoiLut = ((IVoiLutProvider) e.PresentationImage).VoiLutManager.VoiLut as IVoiLutLinear;
					if (sourceVoiLut == null)
						return;

					// find any available display set containing the same series as the individual layers and capture its VOI LUT
					var seriesInstanceUid = ((IImageSopProvider) e.PresentationImage).ImageSop.SeriesInstanceUid;
					foreach (IDisplaySet displaySet in _fusionDisplaySets)
					{
						var anyVisibleChange = false;

						var descriptor = (PETFusionDisplaySetDescriptor) displaySet.Descriptor;
						if (descriptor.SourceSeries.SeriesInstanceUid == seriesInstanceUid)
						{
							// replicate the captured VOI LUT to the fusion images
							foreach (FusionPresentationImage image in displaySet.PresentationImages)
							{
								image.BaseVoiLutManager.InstallVoiLut(ReplicateVoiLut(sourceVoiLut));
								anyVisibleChange |= (image.Visible);
							}
						}
						else if (descriptor.PETSeries.SeriesInstanceUid == seriesInstanceUid)
						{
							// replicate the captured VOI LUT to the fusion images
							foreach (FusionPresentationImage image in displaySet.PresentationImages)
							{
								image.OverlayVoiLutManager.InstallVoiLut(ReplicateVoiLut(sourceVoiLut));
								anyVisibleChange |= (image.Visible);
							}
						}

						// force a draw only if we replicated the VOI LUT to a visible image somewhere
						if (anyVisibleChange)
							displaySet.Draw();
					}
				}
			}
		}

		/// <summary>
		/// Attempts to replicate the specified <paramref name="sourceVoiLut"/>. If the LUT is not linear, computes a dummy LUT.
		/// </summary>
		private static IComposableLut ReplicateVoiLut(IComposableLut sourceVoiLut)
		{
			if (sourceVoiLut is IVoiLutLinear)
			{
				var voiLutLinear = (IVoiLutLinear) sourceVoiLut;
				return new BasicVoiLutLinear(voiLutLinear.WindowWidth, voiLutLinear.WindowCenter);
			}
			return new IdentityVoiLinearLut();
		}

		private static IComposableLut GetInitialVoiLut(Frame frame)
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