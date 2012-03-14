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

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	[ButtonAction("activate", "servertree-toolbar/ToolbarAddServer", "AddNewServer")]
	[MenuAction("activate", "servertree-contextmenu/MenuAddServer", "AddNewServer")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAddServer")]
	[IconSet("activate", "Icons.AddServerToolSmall.png", "Icons.AddServerToolMedium.png", "Icons.AddServerToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class AddServerTool : ServerTreeTool
	{
		public AddServerTool()
		{
		}

		private void AddNewServer()
		{
			Common.ServerTree.ServerTree serverTree = this.Context.ServerTree;
			this.Context.UpdateType = (int)ServerUpdateType.Add;
			DicomServerEditComponent editor = new DicomServerEditComponent(serverTree);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleAddNewServer);
			this.Context.UpdateType = (int)ServerUpdateType.None; 
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			this.Enabled = !this.Context.IsReadOnly && this.Context.ServerTree.CurrentNode.IsServerGroup;
		}
	}
}