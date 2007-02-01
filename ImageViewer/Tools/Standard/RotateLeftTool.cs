using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardRotateLeft")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardRotateLeft", KeyStroke = XKeys.L)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardRotateLeft")]
    [ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardRotateLeft")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RotateLeftMedium.png", "Icons.RotateLeftLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Rotate.Left")]

    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RotateLeftTool : Tool<IImageViewerToolContext>
	{
		public RotateLeftTool()
		{
		}

		public void Activate()
		{
			ISpatialTransformProvider image = this.Context.Viewer.SelectedPresentationImage as ISpatialTransformProvider;

			if (image == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(image);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandRotateLeft;
			command.BeginState = applicator.CreateMemento();

			image.SpatialTransform.Rotation -= 90;

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
