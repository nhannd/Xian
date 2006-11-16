using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
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

	//Mark the delegates as keyboard controllable, without assigning a default keystroke.
	[KeyboardAction("zoomin", "imageviewer-keyboard/ToolsStandardZoom/ZoomIn", KeyStroke = XKeys.OemPeriod)]
	[ClickHandler("zoomin", "ZoomIn")]

	[KeyboardAction("zoomout", "imageviewer-keyboard/ToolsStandardZoom/ZoomOut", KeyStroke = XKeys.Oemcomma)]
	[ClickHandler("zoomout", "ZoomOut")]

	[MouseWheelControl("ZoomIn", "ZoomOut", ModifierFlags.Control)]

	[CursorToken("Icons.ZoomMedium.png", typeof(ZoomTool))]
	[MouseToolButton(XMouseButtons.Right, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ZoomTool : MouseTool
	{
		private UndoableCommand _command;
		private SpatialTransformApplicator _applicator;

		public ZoomTool()
		{
		}

		private void CaptureBeginState(IPresentationImage image)
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
			ITile tile = this.Context.Viewer.SelectedTile;
			if (tile == null || tile.PresentationImage == null)
				return;

			CaptureBeginState(tile.PresentationImage);

			IncrementZoom(tile, 0.2F);

			CaptureEndState();
		}

		private void ZoomOut()
		{
			ITile tile = this.Context.Viewer.SelectedTile;
			if (tile == null || tile.PresentationImage == null)
				return;

			CaptureBeginState(tile.PresentationImage);

			IncrementZoom(tile, -0.2F);

			CaptureEndState();
		}

		private void IncrementZoom(ITile tile, float scaleIncrement)
		{
			if (tile == null || tile.PresentationImage == null)
				return;

			SpatialTransform spatialTransform = tile.PresentationImage.LayerManager.SelectedLayerGroup.SpatialTransform;
			spatialTransform.ScaleToFit = false;
			spatialTransform.Scale += scaleIncrement;
			spatialTransform.Calculate();
			tile.PresentationImage.Draw();
		}

		public override bool Start(MouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (mouseInformation.Tile.PresentationImage == null)
				return true;

			CaptureBeginState(mouseInformation.Tile.PresentationImage);

			return true;
		}

		public override bool Track(MouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			if (_command == null)
				return true;

			IncrementZoom(mouseInformation.Tile, (float)base.DeltaY * 0.025F);

			return true;
		}

		public override bool Stop(MouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);
			
			CaptureEndState();
			
			return true;
		}
	}
}
