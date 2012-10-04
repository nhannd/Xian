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
using ClearCanvas.Desktop.Tools;

// ... (other using namespace statements here)

namespace MyPlugin.Basics
{
	[ButtonAction("open", "global-toolbars/MyTools/ShowMyComponentAsShelf", "ShowMyComponent")]
	[IconSet("open", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class MyShelfTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

		public void ShowMyComponent()
		{
			MyComponent component = new MyComponent();
			_shelf = ApplicationComponent.LaunchAsShelf(
				this.Context.DesktopWindow,
				component,
				"shelf caption",
				ShelfDisplayHint.DockLeft);
			_shelf.Closed += Shelf_Closed;
		}

		/// <summary>
		/// Event handler for IShelf.Closed
		/// </summary>
		private void Shelf_Closed(object sender, ClosedEventArgs e)
		{
			_shelf.Closed -= Shelf_Closed;
			_shelf = null;
		}
	}
}