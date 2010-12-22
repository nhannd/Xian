#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;


namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuFlipHorizontal", "Activate", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuFlipHorizontal", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarFlipHorizontal", "Activate", KeyStroke = XKeys.H)]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipFlipHorizontal")]
	[IconSet("activate", IconScheme.Colour, "Icons.FlipHorizontalToolSmall.png", "Icons.FlipHorizontalToolMedium.png", "Icons.FlipHorizontalToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Flip.Horizontal")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class FlipHorizontalTool : ImageViewerTool
	{
		private readonly SpatialTransformImageOperation _operation;

		public FlipHorizontalTool()
		{
			_operation = new SpatialTransformImageOperation(Apply);
		}

		public void Activate()
		{
			if (!_operation.AppliesTo(this.SelectedPresentationImage))
				return;

			ImageOperationApplicator applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
			UndoableCommand historyCommand = applicator.ApplyToAllImages();
			if (historyCommand != null)
			{
				historyCommand.Name = SR.CommandFlipHorizontal;
				this.Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		#region IImageOperation Members

		public void Apply(IPresentationImage image)
		{
			ISpatialTransform transform = (ISpatialTransform)_operation.GetOriginator(image);
			// Do the transform
			if (transform.RotationXY == 0 || transform.RotationXY == 180)
				transform.FlipY = !transform.FlipY;
			// If image is rotated 90 or 270, then a horizontal flip is really a vertical flip
			else
				transform.FlipX = !transform.FlipX;
		}

		#endregion
	}
}
