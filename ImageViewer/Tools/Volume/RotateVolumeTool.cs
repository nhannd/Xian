using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using vtk;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
    [MenuAction("activate", "imageviewer-contextmenu/Rotate Volume", Flags = ClickActionFlags.CheckAction)]
    //[MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardPan", Flags = ClickActionFlags.CheckAction)]
	[MouseToolButton(XMouseButtons.Left, true)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/RotateVolume", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "Rotate Volume")]
	[IconSet("activate", IconScheme.Colour, "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolLarge.png", "Icons.CreateVolumeToolLarge.png")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RotateVolumeTool : MouseTool
	{
		private CursorToken _cursorToken;

		public RotateVolumeTool()
  		{
			_cursorToken = new CursorToken("Icons.CreateVolumeToolSmall.png", this.GetType().Assembly);
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
			interactor.LeftButtonPressEvent();

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
			interactor.LeftButtonReleaseEvent();

			return false;
		}

		public override void Cancel()
		{
			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return;

			interactor.LeftButtonReleaseEvent();
		}

		public override CursorToken GetCursorToken(Point point)
		{
			return _cursorToken;
		}

		#endregion
	}
}
