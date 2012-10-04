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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;

// ... (other using namespace statements here)

namespace MyPlugin.Basics
{
	[MenuAction("apply", "global-menus/MenuTools/MenuStandard/MenuMyImageViewerTool", "Apply")]
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ToolbarMyImageViewerTool", "Apply")]
	[Tooltip("apply", "TooltipMyImageViewerTool")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class MyImageViewerTool : ImageViewerTool
	{
		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public MyImageViewerTool() {}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			// TODO: add any significant initialization code here rather than in the constructor
		}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Apply()
		{
			// Add code here to implement the functionality of the tool
			if (this.SelectedPresentationImage == null)
				return;
		}
	}
}