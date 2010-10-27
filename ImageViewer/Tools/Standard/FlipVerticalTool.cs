#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuFlipVertical", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarFlipVertical", "Activate", KeyStroke = XKeys.V)]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipFlipVertical")]
	[IconSet("activate", IconScheme.Colour, "Icons.FlipVerticalToolSmall.png", "Icons.FlipVerticalToolMedium.png", "Icons.FlipVerticalToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Flip.Vertical")]

    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class FlipVerticalTool : ImageViewerTool
	{
		private readonly SpatialTransformImageOperation _operation;

		public FlipVerticalTool()
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
				historyCommand.Name = SR.CommandFlipVertical; 
				this.Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		public void Apply(IPresentationImage image)
		{
			ISpatialTransform transform = (ISpatialTransform)_operation.GetOriginator(image);
			// Do the transform
			if (transform.RotationXY == 0 || transform.RotationXY == 180)
				transform.FlipX = !transform.FlipX;
			// If image is rotated 90 or 270, then a vertical flip is really a horizontal flip
			else
				transform.FlipY = !transform.FlipY;
		}
	}
}
