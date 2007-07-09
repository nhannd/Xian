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
	[MenuAction("activate", "imageviewer-contextmenu/MenuZoom", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuZoom", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarZoom", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardZoom/Activate", KeyStroke = XKeys.Z)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ZoomToolSmall.png", "Icons.ZoomToolMedium.png", "Icons.ZoomToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Zoom")]

	//Mark the delegates as keyboard controllable, without assigning a default keystroke.
	[KeyboardAction("zoomin", "imageviewer-keyboard/ToolsStandardZoom/ZoomIn", KeyStroke = XKeys.OemPeriod)]
	[ClickHandler("zoomin", "ZoomIn")]

	[KeyboardAction("zoomout", "imageviewer-keyboard/ToolsStandardZoom/ZoomOut", KeyStroke = XKeys.Oemcomma)]
	[ClickHandler("zoomout", "ZoomOut")]

	[MouseToolWheelControl("ZoomIn", "ZoomOut", ModifierFlags.Control)]
	[MouseToolButton(XMouseButtons.Right, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ZoomTool : MouseImageViewerTool
	{
		private UndoableCommand _command;
		private SpatialTransformApplicator _applicator;

		public ZoomTool()
			: base(SR.TooltipZoom)
		{
			this.CursorToken = new CursorToken("Icons.ZoomToolSmall.png", this.GetType().Assembly);
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
			_command.Name = SR.CommandZoom;
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

		private void ZoomIn()
		{
			CaptureBeginState();

			float increment = 0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementZoom(increment);

			CaptureEndState();
		}

		private void ZoomOut()
		{
			CaptureBeginState();

			float increment = -0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementZoom(increment);

			CaptureEndState();
		}

		private void IncrementZoom(float scaleIncrement)
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			IImageSpatialTransform transform = this.SelectedSpatialTransformProvider.SpatialTransform as IImageSpatialTransform;

			transform.ScaleToFit = false;
			transform.Scale += scaleIncrement;

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

			IncrementZoom((float)base.DeltaY * 0.025F);

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
