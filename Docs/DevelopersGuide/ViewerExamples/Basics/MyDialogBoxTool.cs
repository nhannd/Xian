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
	[ButtonAction("open", "global-toolbars/MyTools/ShowMyComponentAsDialogBox", "ShowMyComponent")]
	[IconSet("open", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class MyDialogBoxTool : Tool<IDesktopToolContext>
	{
		public void ShowMyComponent()
		{
			MyComponent component = new MyComponent();
			SimpleComponentContainer container = new SimpleComponentContainer(component);
			ApplicationComponentExitCode exitCode;
			exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				container,
				"dialog caption");

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				// user clicked OK (or equivalent button)
			}
		}
	}
}