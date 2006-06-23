using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "MenuTools/MenuToolsStandard/MenuToolsStandardReset")]
    [ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardReset")]
    [ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardReset")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.ResetMedium.png", "Icons.ResetLarge.png")]
    
    /// <summary>
	/// Summary description for ResetTool.
	/// </summary>
    //[ClearCanvas.Common.ExtensionOf(typeof(ITool))]
    public class ResetTool : Tool
    {
		public ResetTool()
		{
		}

		private ImageWorkspace Workspace
		{
			get { return (this.Context as ImageWorkspaceToolContext).Workspace; }
		}

		public void Activate()
		{
            PresentationImage selectedImage = ((ImageWorkspaceToolContext)this.Context).Workspace.SelectedPresentationImage;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(selectedImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandReset;
			command.BeginState = applicator.CreateMemento();

			SpatialTransform spatialTransform = selectedImage.LayerManager.SelectedLayerGroup.SpatialTransform;
			spatialTransform.Scale = 1.0f;
			spatialTransform.TranslationX = 0.0f;
			spatialTransform.TranslationY = 0.0f;
			spatialTransform.FlipHorizontal = false;
			spatialTransform.FlipVertical = false;
			spatialTransform.Rotation = 0.0f;
			spatialTransform.ScaleToFit = true;
			spatialTransform.Calculate();

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Workspace.CommandHistory.AddCommand(command);
		}
	}
}
