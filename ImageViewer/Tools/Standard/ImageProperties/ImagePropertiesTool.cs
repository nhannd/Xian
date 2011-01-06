#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarImageProperties", "Show", KeyStroke = XKeys.Control | XKeys.P)]
	[MenuAction("show", "global-menus/MenuView/MenuImageProperties", "Show", KeyStroke = XKeys.Control | XKeys.P)]
	[MenuAction("show", "imageviewer-contextmenu/MenuImageProperties", "Show")]
	[Tooltip("show", "TooltipImageProperties")]
	[IconSet("show", IconScheme.Colour, "ImagePropertiesToolSmall.png", "ImagePropertiesToolMedium.png", "ImagePropertiesToolLarge.png")]
	[GroupHint("show", "Application.View.ImageProperties")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ImagePropertiesTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();
		
		public ImagePropertiesTool()
		{
		}

		private IShelf ComponentShelf
		{
			get
			{
				if (_shelves.ContainsKey(Context.DesktopWindow))
					return _shelves[Context.DesktopWindow];

				return null;
			}
		}

		public void Show()
		{
			if (ComponentShelf == null)
			{
				try
				{
					IDesktopWindow desktopWindow = Context.DesktopWindow;
					
					ImagePropertiesApplicationComponent component =
						new ImagePropertiesApplicationComponent(Context.DesktopWindow);

					IShelf shelf = ApplicationComponent.LaunchAsShelf(Context.DesktopWindow, component,
						SR.TitleImageProperties, "ImageProperties", ShelfDisplayHint.DockLeft);

					_shelves.Add(Context.DesktopWindow, shelf);
					shelf.Closed += delegate { _shelves.Remove(desktopWindow); };
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, Context.DesktopWindow);
				}
			}
			else
			{
				ComponentShelf.Show();
			}
		}
	}
}