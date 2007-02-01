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
	[MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardPan", Flags = ClickActionFlags.CheckAction)]
    [MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardPan", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardPan", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardPan/Activate", KeyStroke = XKeys.P)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardPan")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.PanMedium.png", "Icons.PanLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Pan")]

	[KeyboardAction("panleft", "imageviewer-keyboard/ToolsStandardPan/PanLeft", KeyStroke = XKeys.Control | XKeys.Left)]
	[ClickHandler("panleft", "PanLeft")]

	[KeyboardAction("panright", "imageviewer-keyboard/ToolsStandardPan/PanRight", KeyStroke = XKeys.Control | XKeys.Right)]
	[ClickHandler("panright", "PanRight")]

	[KeyboardAction("panup", "imageviewer-keyboard/ToolsStandardPan/PanUp", KeyStroke = XKeys.Control | XKeys.Up)]
	[ClickHandler("panup", "PanUp")]

	[KeyboardAction("pandown", "imageviewer-keyboard/ToolsStandardPan/PanDown", KeyStroke = XKeys.Control | XKeys.Down)]
	[ClickHandler("pandown", "PanDown")]

	[ModifiedMouseToolButton(XMouseButtons.Left, ModifierFlags.Control)]
	[MouseToolButton(XMouseButtons.Left, false)]
    
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class PanTool : MouseTool
	{
		private UndoableCommand _command;
		private SpatialTransformApplicator _applicator;

		public PanTool()
		{
			this.CursorToken = new CursorToken("Icons.PanMedium.png", this.GetType().Assembly);
		}

		private void CaptureBeginState(ISpatialTransformProvider image)
		{
			_applicator = new SpatialTransformApplicator(image);
			_command = new UndoableCommand(_applicator);
			_command.Name = SR.CommandPan;
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

		private void PanLeft()
		{
			IncrementPan(-15, 0);
		}

		private void PanRight()
		{
			IncrementPan(15, 0);
		}

		private void PanUp()
		{
			IncrementPan(0, -15);
		}

		private void PanDown()
		{
			IncrementPan(0, 15);
		}

		private void IncrementPan(int xIncrement, int yIncrement)
		{
			ISpatialTransformProvider image = this.Context.Viewer.SelectedPresentationImage as ISpatialTransformProvider;

			if (image == null)
				return;

			this.CaptureBeginState(image);
			this.IncrementPan(image, xIncrement, yIncrement);
			this.CaptureEndState();
		}

		private void IncrementPan(ISpatialTransformProvider image, int xIncrement, int yIncrement)
		{
			float scale = image.SpatialTransform.Scale;
			Platform.CheckPositive(scale, "standardImage.SpatialTransform.Scale");

			image.SpatialTransform.TranslationX += xIncrement / scale;
			image.SpatialTransform.TranslationY += yIncrement / scale;
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

			if (_command == null)
				return false;

			this.IncrementPan(image, base.DeltaX, base.DeltaY);

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
