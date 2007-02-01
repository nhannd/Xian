using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardZoom", Flags = ClickActionFlags.CheckAction)]
    [MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardZoom", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardZoom", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardZoom/Activate", KeyStroke = XKeys.Z)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardZoom")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.ZoomMedium.png", "Icons.ZoomLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Zoom")]

	//Mark the delegates as keyboard controllable, without assigning a default keystroke.
	[KeyboardAction("zoomin", "imageviewer-keyboard/ToolsStandardZoom/ZoomIn", KeyStroke = XKeys.OemPeriod)]
	[ClickHandler("zoomin", "ZoomIn")]

	[KeyboardAction("zoomout", "imageviewer-keyboard/ToolsStandardZoom/ZoomOut", KeyStroke = XKeys.Oemcomma)]
	[ClickHandler("zoomout", "ZoomOut")]

	[MouseWheelControl("ZoomIn", "ZoomOut", ModifierFlags.Control)]
	[MouseToolButton(XMouseButtons.Right, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ZoomTool : MouseTool
	{
		private UndoableCommand _command;
		private SpatialTransformApplicator _applicator;

		public ZoomTool()
		{
			this.CursorToken = new CursorToken("Icons.ZoomMedium.png", this.GetType().Assembly);
		}

		private void CaptureBeginState(ISpatialTransformProvider image)
		{
			_applicator = new SpatialTransformApplicator(image);
			_command = new UndoableCommand(_applicator);
			_command.Name = SR.CommandZoom;
			_command.BeginState = _applicator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (_command == null)
				return;

			_command.EndState = _applicator.CreateMemento();

			// If the state hasn't changed since MouseDown just return
			if (_command.EndState.Equals(_command.BeginState))
			{
				_command = null;
				return;
			}

			// Apply the final state to all linked images
			_applicator.SetMemento(_command.EndState);

			this.Context.Viewer.CommandHistory.AddCommand(_command);
		}

		private void ZoomIn()
		{
			ISpatialTransformProvider image = this.Context.Viewer.SelectedPresentationImage as ISpatialTransformProvider;

			if (image == null)
				return;

			CaptureBeginState(image);

			float increment = 0.1F * image.SpatialTransform.Scale;
			IncrementZoom(image, increment);

			CaptureEndState();
		}

		private void ZoomOut()
		{
			ISpatialTransformProvider image = this.Context.Viewer.SelectedPresentationImage as ISpatialTransformProvider;

			if (image == null)
				return;

			CaptureBeginState(image);

			float increment = -0.1F * image.SpatialTransform.Scale;
			IncrementZoom(image, increment);

			CaptureEndState();
		}

		private void IncrementZoom(ISpatialTransformProvider image, float scaleIncrement)
		{
			image.SpatialTransform.ScaleToFit = false;
			image.SpatialTransform.Scale += scaleIncrement;
			image.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			ISpatialTransformProvider image = mouseInformation.Tile.PresentationImage as ISpatialTransformProvider;

			if (image == null)
				return false;

			CaptureBeginState(image);

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			ISpatialTransformProvider image = mouseInformation.Tile.PresentationImage as ISpatialTransformProvider;

			if (image == null)
				return false;

			IncrementZoom(image, (float)base.DeltaY * 0.025F);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();
			
			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}
	}
}
