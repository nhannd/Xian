#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
