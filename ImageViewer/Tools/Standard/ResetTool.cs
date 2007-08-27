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
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuReset")]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarReset")]
    [ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipReset")]
	[IconSet("activate", IconScheme.Colour, "Icons.ResetToolSmall.png", "Icons.ResetToolMedium.png", "Icons.ResetToolLarge.png")]
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

			applicator.ApplyToAllImages(delegate(IPresentationImage presentationImage)
			{
				ISpatialTransformProvider image = presentationImage as ISpatialTransformProvider;
				if (image == null)
					return;

				IImageSpatialTransform transform = image.SpatialTransform as IImageSpatialTransform;
				if (transform == null)
					return;

				transform.Scale = 1.0f;
				transform.TranslationX = 0.0f;
				transform.TranslationY = 0.0f;
				transform.FlipY = false;
				transform.FlipX = false;
				transform.RotationXY = 0;
				transform.ScaleToFit = true;
			});

			command.EndState = applicator.CreateMemento();

            this.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
