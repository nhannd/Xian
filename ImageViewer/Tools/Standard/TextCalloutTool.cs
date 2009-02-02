using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuAnnotation/MenuTextCallout", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarAnnotation/ToolbarTextCallout", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.TextCalloutToolSmall.png", "Icons.TextCalloutToolMedium.png", "Icons.TextCalloutToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Annotation.TextCallout")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class TextCalloutTool : MouseImageViewerTool
	{
		private DrawableUndoableCommand _undoableCommand;
		private ArrowInteractiveGraphic _arrow;

		public TextCalloutTool() : base(SR.TooltipTextCallout)
		{
			this.Behaviour = MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		public string CreationCommandName
		{
			get { return SR.CommandCreateTextCallout; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_arrow != null)
				return _arrow.Start(mouseInformation);

			IPresentationImage image = mouseInformation.Tile.PresentationImage;
			IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return false;

			_arrow = new ArrowInteractiveGraphic(true);

			_undoableCommand = new DrawableUndoableCommand(image);
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(_arrow, provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Name = this.CreationCommandName;
			_undoableCommand.Execute();

			if (_arrow.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_arrow != null)
				return _arrow.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_arrow == null)
				return false;

			if (_arrow.Stop(mouseInformation))
				return true;

			_arrow.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_undoableCommand = null;
			_arrow = null;
			return false;
		}

		public override void Cancel()
		{
			if (_arrow == null)
				return;

			_arrow.Cancel();

			_undoableCommand.Unexecute();
			_undoableCommand = null;

			_arrow = null;
		}
	}
}