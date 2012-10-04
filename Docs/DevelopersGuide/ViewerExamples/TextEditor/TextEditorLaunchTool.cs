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

namespace MyPlugin.TextEditor
{
	[MenuAction("launch", "global-menus/Tools/Text Editor", "Launch")]
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class TextEditorLaunchTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;
		private IApplicationComponent _component;

		public TextEditorLaunchTool() : base() {}

		public void Launch()
		{
			_component = new TextEditorComponent();
			_workspace = ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				_component,
				"Text Editor");

			_workspace.Closed += Workspace_Closed;
		}

		protected override void Dispose(bool disposing)
		{
			if (_workspace != null)
				_workspace.Closed -= Workspace_Closed;

			base.Dispose(disposing);
		}

		private void Workspace_Closed(object sender, ClosedEventArgs e)
		{
			// Check the exit code if necessary
			if (_component.ExitCode == ApplicationComponentExitCode.Accepted)
			{
				// Do something
			}
		}
	}
}