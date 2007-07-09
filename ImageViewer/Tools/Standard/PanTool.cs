using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuPan", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuPan", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarPan", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardPan/Activate", KeyStroke = XKeys.P)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.PanToolSmall.png", "Icons.PanToolMedium.png", "Icons.PanToolLarge.png")]
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
	public class PanTool : MouseImageViewerTool
	{
		private UndoableCommand _command;
		private SpatialTransformApplicator _applicator;

		public PanTool()
			: base(SR.TooltipPan)
		{
			this.CursorToken = new CursorToken("Icons.PanToolSmall.png", this.GetType().Assembly);
		}

		public override string Tooltip
		{
			get { return base.Tooltip; }
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private void CaptureBeginState()
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			_applicator = new SpatialTransformApplicator(this.SelectedPresentationImage);
			_command = new UndoableCommand(_applicator);
			_command.Name = SR.CommandPan;
			_command.BeginState = _applicator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			if (_command == null)
				return;

			_applicator.ApplyToLinkedImages();
			_command.EndState = _applicator.CreateMemento();

			// If the state hasn't changed since MouseDown just return
			if (_command.EndState.Equals(_command.BeginState))
			{
				_command = null;
				return;
			}

			this.Context.Viewer.CommandHistory.AddCommand(_command);
		}

		private void PanLeft()
		{
			IncrementPanWithUndo(-15, 0);
		}

		private void PanRight()
		{
			IncrementPanWithUndo(15, 0);
		}

		private void PanUp()
		{
			IncrementPanWithUndo(0, -15);
		}

		private void PanDown()
		{
			IncrementPanWithUndo(0, 15);
		}

		private void IncrementPanWithUndo(int xIncrement, int yIncrement)
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			this.CaptureBeginState();
			this.IncrementPan(xIncrement, yIncrement);
			this.CaptureEndState();
		}

		private void IncrementPan(int xIncrement, int yIncrement)
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			SpatialTransform transform = this.SelectedSpatialTransformProvider.SpatialTransform as SpatialTransform;

			// Because the pan increment is in destination coordinates, we have to convert
			// them to source coordinates, since the transform translation is in source coordinates.
			// This will allow the pan to work properly irrespective of the zoom, flip and rotation.
			PointF[] pt = {new PointF(xIncrement, yIncrement)};
			transform.ConvertVectorsToSource(pt);

			transform.TranslationX += pt[0].X;
			transform.TranslationY += pt[0].Y;

			this.SelectedSpatialTransformProvider.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			CaptureBeginState();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			this.IncrementPan(base.DeltaX, base.DeltaY);

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
