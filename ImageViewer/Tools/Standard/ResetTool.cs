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
			ISpatialTransformProvider image = this.SelectedSpatialTransformProvider;

			if (image == null)
				return;

			SpatialTransformApplicator applicator = new SpatialTransformApplicator(image);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandReset;
			command.BeginState = applicator.CreateMemento();

			image.SpatialTransform.Scale = 1.0f;
			image.SpatialTransform.TranslationX = 0.0f;
			image.SpatialTransform.TranslationY = 0.0f;
			image.SpatialTransform.FlipHorizontal = false;
			image.SpatialTransform.FlipVertical = false;
			image.SpatialTransform.Rotation = 0;
			image.SpatialTransform.ScaleToFit = true;

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
