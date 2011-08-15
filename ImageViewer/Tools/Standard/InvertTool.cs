#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuInvert", "Apply", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuInvert", "Apply")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarInvert", "Apply", KeyStroke = XKeys.I)]
	[Tooltip("activate", "TooltipInvert")]
	[IconSet("activate", IconScheme.Colour, "Icons.InvertToolSmall.png", "Icons.InvertToolMedium.png", "Icons.InvertToolLarge.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [GroupHint("activate", "Tools.Image.Manipulation.Invert")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class InvertTool : ImageViewerTool
	{
		private readonly VoiLutImageOperation _operation;

		public InvertTool()
		{
			_operation = new VoiLutImageOperation(Invert);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.Enabled = _operation.GetOriginator(e.SelectedPresentationImage) != null;
		}

		public void Apply()
		{
			if (_operation.GetOriginator(this.Context.Viewer.SelectedPresentationImage) == null)
				return;

			ImageOperationApplicator applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			UndoableCommand historyCommand = applicator.ApplyToAllImages();
			if (historyCommand != null)
			{
				historyCommand.Name = SR.CommandInvert;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		private void Invert(IPresentationImage image)
		{
			IVoiLutManager manager = (IVoiLutManager)_operation.GetOriginator(image);
			manager.Invert = !manager.Invert;
		}
	}
}
