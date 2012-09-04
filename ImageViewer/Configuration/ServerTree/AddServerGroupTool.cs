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
	[ButtonAction("activate", "servertree-toolbar/ToolbarAddServerGroup", "AddNewServerGroup")]
	[MenuAction("activate", "servertree-contextmenu/MenuAddServerGroup", "AddNewServerGroup")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAddServerGroup")]
	[IconSet("activate", "Icons.AddServerGroupToolSmall.png", "Icons.AddServerGroupToolMedium.png", "Icons.AddServerGroupToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class AddServerGroupTool : ServerTreeTool
	{
		public AddServerGroupTool()
		{
		}

		private void AddNewServerGroup()
		{
			ServerTree _serverTree = this.Context.ServerTree;
			this.Context.UpdateType = (int)ServerUpdateType.Add;
			DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(_serverTree, ServerUpdateType.Add);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleAddServerGroup);
			this.Context.UpdateType = (int)ServerUpdateType.None; 
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			this.Enabled = !this.Context.IsReadOnly && this.Context.ServerTree.CurrentNode.IsServerGroup;
		}
	}
}