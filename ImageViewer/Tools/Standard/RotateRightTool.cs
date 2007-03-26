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
	[MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardRotateRight")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardRotateRight", KeyStroke = XKeys.R)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardRotateRight")]
    [ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "ToolbarToolsStandardRotateRight")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RotateRightMedium.png", "Icons.RotateRightLarge.png")]
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

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
