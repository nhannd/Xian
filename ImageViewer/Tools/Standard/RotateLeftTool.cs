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
    
    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RotateLeftTool : Tool<IImageViewerToolContext>
	{
		public RotateLeftTool()
		{
		}

		public void Activate()
		{
			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			if (selectedImage == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(selectedImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandRotateLeft;
			command.BeginState = applicator.CreateMemento();

			SpatialTransform spatialTransform = selectedImage.LayerManager.SelectedLayerGroup.SpatialTransform;
			spatialTransform.Rotation = spatialTransform.Rotation - 90;
			spatialTransform.Calculate();

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
