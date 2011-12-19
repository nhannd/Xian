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
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuReset", "Activate", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuReset", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarReset", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipReset")]
	[IconSet("activate", IconScheme.Colour, "Icons.ResetToolSmall.png", "Icons.ResetToolMedium.png", "Icons.ResetToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Reset")]

	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ResetTool : ImageViewerTool
    {
		private readonly ImageSpatialTransformImageOperation _operation;
		private ToolModalityBehaviorHelper _toolBehavior;
		
		public ResetTool()
		{
			_operation = new ImageSpatialTransformImageOperation(Apply);
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		public void Activate()
		{
			if (!_operation.AppliesTo(this.SelectedPresentationImage))
				return;

			ImageOperationApplicator applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			UndoableCommand historyCommand = _toolBehavior.Behavior.SelectedImageResetTool ? applicator.ApplyToReferenceImage() : applicator.ApplyToAllImages();
			if (historyCommand != null)
			{
				historyCommand.Name = SR.CommandReset;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		public void Apply(IPresentationImage image)
		{
			IImageSpatialTransform transform = (IImageSpatialTransform)_operation.GetOriginator(image);
			transform.Scale = 1.0f;
			transform.TranslationX = 0.0f;
			transform.TranslationY = 0.0f;
			transform.FlipY = false;
			transform.FlipX = false;
			transform.RotationXY = 0;
			transform.ScaleToFit = true;
		}
	}
}
