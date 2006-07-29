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
    [MenuAction("activate", "MenuTools/MenuToolsStandard/MenuToolsStandardFlipVertical")]
    [ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardFlipVertical")]
    [ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardFlipVertical")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.FlipVerticalMedium.png", "Icons.FlipVerticalLarge.png")]
    
    /// <summary>
	/// Summary description for FlipVerticalTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceToolExtensionPoint))]
    public class FlipVerticalTool : Tool
	{
		public FlipVerticalTool()
		{
		}

		private ImageWorkspace Workspace
		{
			get { return (this.Context as ImageWorkspaceToolContext).Workspace; }
		}

		public void Activate()
		{
            PresentationImage selectedImage = ((ImageWorkspaceToolContext)this.Context).Workspace.SelectedPresentationImage;

			if (selectedImage == null)
				return;

			// Save the old state
			SpatialTransformApplicator applicator = new SpatialTransformApplicator(selectedImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandFlipVertical;
			command.BeginState = applicator.CreateMemento();

			SpatialTransform spatialTransform = selectedImage.LayerManager.SelectedLayerGroup.SpatialTransform;

			// Do the transform
			if (spatialTransform.Rotation == 0 || spatialTransform.Rotation == 180)
				spatialTransform.FlipVertical = !spatialTransform.FlipVertical;
			// If image is rotated 90 or 270, then a vertical flip is really a horizontal flip
			else
				spatialTransform.FlipHorizontal = !spatialTransform.FlipHorizontal;

			spatialTransform.Calculate();

			// Save the new state
			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Workspace.CommandHistory.AddCommand(command);
		}
	}
}
