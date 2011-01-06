#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuRotateLeft", "Activate", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuRotateLeft", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarRotateLeft", "Activate", KeyStroke = XKeys.L)]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRotateLeft")]
	[IconSet("activate", IconScheme.Colour, "Icons.RotateLeftToolSmall.png", "Icons.RotateLeftToolMedium.png", "Icons.RotateLeftToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Rotate.Left")]

    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RotateLeftTool : ImageViewerTool
	{
		private readonly SpatialTransformImageOperation _operation;
		
		public RotateLeftTool()
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
				historyCommand.Name = SR.CommandRotateLeft;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		public void Apply(IPresentationImage image)
		{
			ISpatialTransform transform = (ISpatialTransform)_operation.GetOriginator(image);
			transform.RotationXY -= 90;
		}
	}
}
