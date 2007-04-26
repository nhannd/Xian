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
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuRotateLeft")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardRotateLeft", KeyStroke = XKeys.L)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarRotateLeft")]
    [ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRotateLeft")]
	[IconSet("activate", IconScheme.Colour, "Icons.RotateLeftToolSmall.png", "Icons.RotateLeftToolMedium.png", "Icons.RotateLeftToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Rotate.Left")]

    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RotateLeftTool : ImageViewerTool
	{
		public RotateLeftTool()
		{
		}

		public void Activate()
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(this.SelectedPresentationImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandRotateLeft;
			command.BeginState = applicator.CreateMemento();

			this.SelectedSpatialTransformProvider.SpatialTransform.RotationXY -= 90;

			applicator.ApplyToLinkedImages();
			command.EndState = applicator.CreateMemento();

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
