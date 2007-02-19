using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "global-menus/MenuTools/MenuToolsStandard/MenuToolsStandardReset")]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardReset")]
    [ClickHandler("activate", "Activate")]
    [Tooltip("activate", "ToolbarToolsStandardReset")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.ResetMedium.png", "Icons.ResetLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Reset")]

    //[ClearCanvas.Common.ExtensionOf(typeof(ITool))]
    public class ResetTool : ImageViewerTool
    {
		public ResetTool()
		{
		}

		public void Activate()
		{
			if (this.SelectedPresentationImage == null ||
				this.SelectedSpatialTransformProvider == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(this.SelectedPresentationImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandReset;
			command.BeginState = applicator.CreateMemento();

			this.SelectedSpatialTransformProvider.SpatialTransform.Scale = 1.0f;
			this.SelectedSpatialTransformProvider.SpatialTransform.TranslationX = 0.0f;
			this.SelectedSpatialTransformProvider.SpatialTransform.TranslationY = 0.0f;
			this.SelectedSpatialTransformProvider.SpatialTransform.FlipHorizontal = false;
			this.SelectedSpatialTransformProvider.SpatialTransform.FlipVertical = false;
			this.SelectedSpatialTransformProvider.SpatialTransform.Rotation = 0;
			this.SelectedSpatialTransformProvider.SpatialTransform.ScaleToFit = true;

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
