using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "global-menus/MenuTools/MenuToolsStandard/MenuToolsStandardFlipHorizontal")]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardFlipHorizontal")]
    [ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardFlipHorizontal")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.FlipHorizontalMedium.png", "Icons.FlipHorizontalLarge.png")]
    
    /// <summary>
	/// Summary description for FlipHorizontalTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class FlipHorizontalTool : ImageViewerTool
	{
		public FlipHorizontalTool()
		{
		}

		public void Activate()
		{
            PresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			if (selectedImage == null)
				return;

			// Save the old state
			SpatialTransformApplicator applicator = new SpatialTransformApplicator(selectedImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandFlipHorizontal;
			command.BeginState = applicator.CreateMemento();

			SpatialTransform spatialTransform = selectedImage.LayerManager.SelectedLayerGroup.SpatialTransform;

			// Do the transform
			if (spatialTransform.Rotation == 0 || spatialTransform.Rotation == 180)
				spatialTransform.FlipHorizontal = !spatialTransform.FlipHorizontal;
			// If image is rotated 90 or 270, then a horizontal flip is really a vertical flip
			else
				spatialTransform.FlipVertical = !spatialTransform.FlipVertical;

			// Save the new state
			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
