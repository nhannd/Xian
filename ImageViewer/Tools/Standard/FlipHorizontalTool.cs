using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;


namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuFlipHorizontal", "Activate")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardFlipHorizontal", "Activate", KeyStroke = XKeys.H)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarFlipHorizontal", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipFlipHorizontal")]
	[IconSet("activate", IconScheme.Colour, "Icons.FlipHorizontalToolSmall.png", "Icons.FlipHorizontalToolMedium.png", "Icons.FlipHorizontalToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Flip.Horizontal")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
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

			applicator.ApplyToAllImages(
				delegate(IPresentationImage presentationImage)
				{
					ISpatialTransformProvider image = presentationImage as ISpatialTransformProvider;
					if (image == null)
						return;

					// Do the transform
					if (image.SpatialTransform.RotationXY == 0 || image.SpatialTransform.RotationXY == 180)
						image.SpatialTransform.FlipY = !image.SpatialTransform.FlipY;
					// If image is rotated 90 or 270, then a horizontal flip is really a vertical flip
					else
						image.SpatialTransform.FlipX = !image.SpatialTransform.FlipX;
				});

			// Save the new state
			command.EndState = applicator.CreateMemento();

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
