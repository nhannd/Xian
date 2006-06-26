using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    public abstract class ZoomFixedTool : Tool
    {
        public ZoomFixedTool()
		{
		}

		private ImageWorkspace Workspace
		{
			get { return (this.Context as ImageWorkspaceToolContext).Workspace; }
		}
		
		public abstract void Activate();

        protected void ApplyZoom(float scale)
        {
            PresentationImage selectedImage = ((ImageWorkspaceToolContext)this.Context).Workspace.SelectedPresentationImage;

            if (selectedImage == null)
                return;

            SpatialTransformApplicator applicator = new SpatialTransformApplicator(selectedImage);
            UndoableCommand command = new UndoableCommand(applicator);
            command.Name = SR.CommandZoom;
            command.BeginState = applicator.CreateMemento();

            SpatialTransform spatialTransform = selectedImage.LayerManager.SelectedLayerGroup.SpatialTransform;
            spatialTransform.Scale = scale;
            spatialTransform.Calculate();

            command.EndState = applicator.CreateMemento();

            // Apply the final state to all linked images
            applicator.SetMemento(command.EndState);

            this.Workspace.CommandHistory.AddCommand(command);
        }
   }
}
