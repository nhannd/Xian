#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuCine", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarCine", "Activate", KeyStroke = XKeys.C)]
	[Tooltip("activate", "TooltipCine")]
	[IconSet("activate", IconScheme.Colour, "Icons.CineToolSmall.png", "Icons.CineToolMedium.png", "Icons.CineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Cine")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class CineTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();

		public CineTool() {}

		public void Activate()
		{
			IDesktopWindow desktopWindow = this.Context.DesktopWindow;

			// check if a layout component is already displayed
			if (_shelves.ContainsKey(desktopWindow))
			{
				_shelves[desktopWindow].Activate();
			}
			else
			{
				LaunchShelf(desktopWindow, new CineApplicationComponent(desktopWindow), ShelfDisplayHint.DockFloat);
			}
		}

		protected IImageBox SelectedImageBox
		{
			get
			{
				if (this.ImageViewer == null)
					return null;
				return this.ImageViewer.SelectedImageBox;
			}
		}

		private static void LaunchShelf(IDesktopWindow desktopWindow, IApplicationComponent component, ShelfDisplayHint shelfDisplayHint)
		{
			IShelf shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleCine, "Cine", shelfDisplayHint);
			_shelves[desktopWindow] = shelf;
			_shelves[desktopWindow].Closed += OnShelfClosed;
		}

		private static void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			// We need to cache the owner DesktopWindow (_desktopWindow) because this tool is an 
			// ImageViewer tool, disposed when the viewer component is disposed.  Shelves, however,
			// exist at the DesktopWindow level and there can only be one of each type of shelf
			// open at the same time per DesktopWindow (otherwise things look funny).  Because of 
			// this, we need to allow this event handling method to be called after this tool has
			// already been disposed (e.g. viewer workspace closed), which is why we store the 
			// _desktopWindow variable.

			IShelf shelf = (IShelf) sender;
			shelf.Closed -= OnShelfClosed;
			_shelves.Remove(shelf.DesktopWindow);
		}
	}
}