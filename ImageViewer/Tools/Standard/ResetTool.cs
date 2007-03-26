using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "global-menus/MenuTools/MenuToolsStandard/MenuToolsStandardReset")]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardReset")]
    [ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "ToolbarToolsStandardReset")]
	[IconSet("activate", IconScheme.Colour, "Icons.ResetSmall.png", "Icons.ResetMedium.png", "Icons.ResetLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Reset")]

	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
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

			IImageSpatialTransform transform = this.SelectedSpatialTransformProvider.SpatialTransform as IImageSpatialTransform;

			transform.Scale = 1.0f;
			transform.TranslationX = 0.0f;
			transform.TranslationY = 0.0f;
			transform.FlipY = false;
			transform.FlipX = false;
			transform.RotationXY = 0;
			transform.ScaleToFit = true;
			this.SelectedSpatialTransformProvider.Draw();

			command.EndState = applicator.CreateMemento();

			// Apply the final state to all linked images
			applicator.SetMemento(command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
