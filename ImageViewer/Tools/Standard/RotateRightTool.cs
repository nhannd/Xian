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
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuRotateRight", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarRotateRight", "Activate", KeyStroke = XKeys.R)]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRotateRight")]
	[IconSet("activate", IconScheme.Colour, "Icons.RotateRightToolSmall.png", "Icons.RotateRightToolMedium.png", "Icons.RotateRightToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Rotate.Right")]

    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RotateRightTool : ImageViewerTool
	{
		private readonly SpatialTransformImageOperation _operation;

		public RotateRightTool()
		{
			_operation = new SpatialTransformImageOperation(Apply);
		}

		public void Activate()
		{
			if (!_operation.AppliesTo(this.SelectedPresentationImage))
				return;

			ImageOperationApplicator applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			UndoableCommand historyCommand = applicator.ApplyToAllImages();
			if (historyCommand != null)
			{
				historyCommand.Name = SR.CommandRotateRight;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		public void Apply(IPresentationImage image)
		{
			ISpatialTransform transform = (ISpatialTransform)_operation.GetOriginator(image);
			transform.RotationXY += 90;
		}
	}
}
