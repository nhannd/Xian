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
	[ButtonAction("open", "global-toolbars/MyTools/ShowMyComponentAsWorkspace", "ShowMyComponent")]
	[IconSet("open", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class MyWorkspaceTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void ShowMyComponent()
		{
			MyComponent component = new MyComponent();
			_workspace = ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				component,
				"workspace caption");
			_workspace.Closed += Workspace_Closed;
		}

		/// <summary>
		/// Event handler for IWorkspace.Closed
		/// </summary>
		private void Workspace_Closed(object sender, ClosedEventArgs e)
		{
			_workspace.Closed -= Workspace_Closed;
			_workspace = null;
		}
	}
}