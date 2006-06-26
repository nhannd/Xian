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
    [MenuAction("activate", "MenuTools/MenuToolsStandard/MenuToolsStandardRotateLeft")]
    [ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardRotateLeft")]
    [ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardRotateLeft")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RotateLeftMedium.png", "Icons.RotateLeftLarge.png")]
    
    /// <summary>
	/// Summary description for RotateLeftTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceToolExtensionPoint))]
    public class RotateLeftTool : Tool
	{
		public RotateLeftTool()
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
			command.Name = SR.CommandRotateLeft;
			command.BeginState = applicator.CreateMemento();

			SpatialTransform spatialTransform = selectedImage.LayerManager.SelectedLayerGroup.SpatialTransform;
			spatialTransform.Rotation = spatialTransform.Rotation - 90;
			spatialTransform.Calculate();

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Workspace.CommandHistory.AddCommand(command);
		}
	}
}
