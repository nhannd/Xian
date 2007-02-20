using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    public abstract class ZoomFixedTool : ImageViewerTool
    {
        public ZoomFixedTool()
		{
		}

		public abstract void Activate();

        protected void ApplyZoom(float scale)
        {
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(this.SelectedPresentationImage);
            UndoableCommand command = new UndoableCommand(applicator);
            command.Name = SR.CommandZoom;
            command.BeginState = applicator.CreateMemento();

			this.SelectedSpatialTransformProvider.SpatialTransform.Scale = scale;

            command.EndState = applicator.CreateMemento();

            // Apply the final state to all linked images
            applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
        }
   }
}
