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
    public abstract class ZoomFixedTool : Tool<IImageViewerToolContext>
    {
        public ZoomFixedTool()
		{
		}

		public abstract void Activate();

        protected void ApplyZoom(float scale)
        {
			ISpatialTransformProvider image = this.Context.Viewer.SelectedPresentationImage as ISpatialTransformProvider;

			if (image == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(image);
            UndoableCommand command = new UndoableCommand(applicator);
            command.Name = SR.CommandZoom;
            command.BeginState = applicator.CreateMemento();

			image.SpatialTransform.Scale = scale;

            command.EndState = applicator.CreateMemento();

            // Apply the final state to all linked images
            applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
        }
   }
}
