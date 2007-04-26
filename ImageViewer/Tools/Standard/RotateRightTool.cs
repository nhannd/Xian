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
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuRotateRight")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardRotateRight", KeyStroke = XKeys.R)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarRotateRight")]
    [ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRotateRight")]
	[IconSet("activate", IconScheme.Colour, "Icons.RotateRightToolSmall.png", "Icons.RotateRightToolMedium.png", "Icons.RotateRightToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Rotate.Right")]

    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RotateRightTool : ImageViewerTool
	{
		public RotateRightTool()
		{
		}

		public void Activate()
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(this.SelectedPresentationImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandRotateRight;
			command.BeginState = applicator.CreateMemento();

			this.SelectedSpatialTransformProvider.SpatialTransform.RotationXY += 90;

			applicator.ApplyToLinkedImages();
			command.EndState = applicator.CreateMemento();

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
