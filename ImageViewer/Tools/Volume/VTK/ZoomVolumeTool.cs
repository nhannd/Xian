#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using vtk;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	[MouseToolButton(XMouseButtons.Right, false)]
	[MenuAction("activate", "imageviewer-contextmenu/Zoom Volume", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarsVolume/ZoomVolume", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [Tooltip("activate", "Zoom Volume")]
	[IconSet("activate", IconScheme.Colour, "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolLarge.png", "Icons.CreateVolumeToolLarge.png")]
	[GroupHint("activate", "Tools.VolumeImage.Manipulation.Zoom")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ZoomVolumeTool : MouseImageViewerTool
	{
		public ZoomVolumeTool()
  		{
			this.CursorToken = new CursorToken("Icons.CreateVolumeToolSmall.png", this.GetType().Assembly);
		}

		vtkGenericRenderWindowInteractor GetInteractor(IPresentationImage selectedImage)
		{
			if (selectedImage == null)
				return null;

			VolumePresentationImage image = selectedImage as VolumePresentationImage;

			if (image == null)
				return null;

			VolumePresentationImageRenderer renderer = image.ImageRenderer as VolumePresentationImageRenderer;

			if (renderer == null)
				return null;

			vtkGenericRenderWindowInteractor interactor = vtkGenericRenderWindowInteractor.SafeDownCast(renderer.Interactor);

			return interactor;
		}

		#region IMouseButtonHandler Members

		public override bool Start(IMouseInformation mouseInformation)
		{
		 	base.Start(mouseInformation);

			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return false;

			interactor.SetEventPositionFlipY(mouseInformation.Location.X, mouseInformation.Location.Y);
			interactor.RightButtonPressEvent();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return false;

			interactor.SetEventPositionFlipY(mouseInformation.Location.X, mouseInformation.Location.Y);
			interactor.MouseMoveEvent();

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return false;

			interactor.SetEventPositionFlipY(mouseInformation.Location.X, mouseInformation.Location.Y);
			interactor.RightButtonReleaseEvent();

			return false;
		}

		public override void Cancel()
		{
			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return;

			interactor.RightButtonReleaseEvent();
		}

		#endregion
	}
}
