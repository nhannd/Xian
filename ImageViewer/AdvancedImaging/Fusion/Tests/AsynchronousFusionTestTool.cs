#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[MenuAction("alpha", "global-menus/MenuDebug/MenuFusion/(Async) Unload PET Volume", "DropThisVolume")]
	[MenuAction("bravo", "global-menus/MenuDebug/MenuFusion/(Async) Unload Selected Fused Image", "DropThisSlice")]
	[MenuAction("charlie", "global-menus/MenuDebug/MenuFusion/(Async) Unload Fused Display Set", "DropAllSlices")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class AsynchronousFusionTestTool : ImageViewerTool
	{
		public void DropThisVolume()
		{
			var image = base.SelectedPresentationImage as FusionPresentationImage;
			if (image != null)
			{
				Unload(image.OverlayFrameData.OverlayData);
			}
		}

		public void DropThisSlice()
		{
			var image = base.SelectedPresentationImage as FusionPresentationImage;
			if (image != null)
			{
				Unload(image.OverlayFrameData);
			}
		}

		public void DropAllSlices()
		{
			var image = base.SelectedPresentationImage as FusionPresentationImage;
			if (image != null)
			{
				foreach (FusionPresentationImage singleImage in base.SelectedPresentationImage.ParentDisplaySet.PresentationImages)
				{
					Unload(singleImage.OverlayFrameData);
				}
			}
		}

		private static void Unload(ILargeObjectContainer loc)
		{
			loc.Unload();
		}
	}
}

#endif