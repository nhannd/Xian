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
    [MenuAction("activate", "MenuTools/MenuToolsStandard/MenuToolsStandardFlipHorizontal")]
    [ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardFlipHorizontal")]
    [ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardFlipHorizontal")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.FlipHorizontalMedium.png", "Icons.FlipHorizontalLarge.png")]
    
    /// <summary>
	/// Summary description for FlipHorizontalTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceToolExtensionPoint))]
	public class FlipHorizontalTool : Tool
	{
		public FlipHorizontalTool()
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

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(selectedImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandFlipHorizontal;
			command.BeginState = applicator.CreateMemento();

			SpatialTransform spatialTransform = selectedImage.LayerManager.SelectedLayerGroup.SpatialTransform;
			spatialTransform.FlipHorizontal = !spatialTransform.FlipHorizontal;

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Workspace.CommandHistory.AddCommand(command);
		}
	}
}
