#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Configuration
{
	[MenuAction("customize", "global-menus/MenuTools/MenuCustomizeActionModels", "Customize")]
	[GroupHint("customize", "Application.Options.Customize")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class CustomizeViewerActionModelTool : ImageViewerTool
	{
		public void Customize()
		{
			try
			{
				CustomizeViewerActionModelsComponent component = new CustomizeViewerActionModelsComponent(this.ImageViewer);

				DialogBoxCreationArgs args = new DialogBoxCreationArgs(component, SR.TitleCustomizeActionModels, "CustomizeActionModels")
				                             	{
				                             		AllowUserResize = true
				                             	};
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, args);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Context.DesktopWindow);
			}
		}
	}
}