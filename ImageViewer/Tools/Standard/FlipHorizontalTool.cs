using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;


namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardFlipHorizontal")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardFlipHorizontal", KeyStroke = XKeys.H)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardFlipHorizontal")]
	[ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardFlipHorizontal")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.FlipHorizontalMedium.png", "Icons.FlipHorizontalLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Flip.Horizontal")]

	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class FlipHorizontalTool : ImageViewerTool
	{
		public FlipHorizontalTool()
		{
		}

		public void Activate()
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			// Save the old state
			SpatialTransformApplicator applicator = new SpatialTransformApplicator(this.SelectedPresentationImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandFlipHorizontal;
			command.BeginState = applicator.CreateMemento();

			ISpatialTransformProvider image = this.SelectedSpatialTransformProvider;

			// Do the transform
			if (image.SpatialTransform.Rotation == 0 || image.SpatialTransform.Rotation == 180)
				image.SpatialTransform.FlipHorizontal = !image.SpatialTransform.FlipHorizontal;
			// If image is rotated 90 or 270, then a horizontal flip is really a vertical flip
			else
				image.SpatialTransform.FlipVertical = !image.SpatialTransform.FlipVertical;

			// Save the new state
			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
