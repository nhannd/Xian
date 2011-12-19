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
using ClearCanvas.ImageViewer.Thumbnails.Configuration;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	[MenuAction("show", "global-menus/MenuView/MenuShowThumbnails", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowThumbnails", "Show")]
	[Tooltip("show", "TooltipShowThumbnails")]
	[IconSet("show", IconScheme.Colour, "Icons.ShowThumbnailsToolSmall.png", "Icons.ShowThumbnailsToolMedium.png", "Icons.ShowThumbnailsToolLarge.png")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShowThumbnailsTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();

		public ShowThumbnailsTool()
		{
		}

		private IShelf ComponentShelf
		{
			get
			{
				if (_shelves.ContainsKey(this.Context.DesktopWindow))
					return _shelves[this.Context.DesktopWindow];

				return null;
			}	
		}

		public void Show()
		{
			if (ComponentShelf == null)
			{
				try
				{
					IDesktopWindow desktopWindow = this.Context.DesktopWindow;

					IShelf shelf = ThumbnailComponent.Launch(desktopWindow);
					shelf.Closed += delegate
					                	{
					                		_shelves.Remove(desktopWindow);
					                	};

					_shelves[this.Context.DesktopWindow] = shelf;
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
			else
			{
				ComponentShelf.Show();
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			if (ThumbnailsSettings.Default.AutoOpenThumbnails)
			{
				this.Show();
			}
		}
	}
}
