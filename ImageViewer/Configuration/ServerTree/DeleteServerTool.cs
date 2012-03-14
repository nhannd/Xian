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
	[ButtonAction("activate", "servertree-toolbar/ToolbarDelete", "DeleteServerServerGroup")]
	[MenuAction("activate", "servertree-contextmenu/MenuDelete", "DeleteServerServerGroup")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDelete")]
	[IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class DeleteServerTool : ServerTreeTool
	{
		public DeleteServerTool()
		{
		}

		private void DeleteServerServerGroup()
		{
			Common.ServerTree.ServerTree serverTree = this.Context.ServerTree;
			if (serverTree.CurrentNode.IsServer)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteServer, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
					return;

				this.Context.UpdateType = (int)ServerUpdateType.Delete;
				serverTree.DeleteDicomServer();
				this.Context.UpdateType = (int)ServerUpdateType.None; 
			}
			else if (serverTree.CurrentNode.IsServerGroup)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteServerGroup, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
					return;

				this.Context.UpdateType = (int)ServerUpdateType.Delete;
				serverTree.DeleteServerGroup();
				this.Context.UpdateType = (int)ServerUpdateType.None; 
			}
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			if (this.Context.IsReadOnly)
			{
				this.Enabled = false;
			}
			else
			{
				//enable only if it's a server or server group, and is not the "My Servers" root node.
				this.Enabled = (this.Context.ServerTree.CurrentNode.IsServer || this.Context.ServerTree.CurrentNode.IsServerGroup) &&
				               this.Context.ServerTree.CurrentNode != this.Context.ServerTree.RootNode.ServerGroupNode;
			}
		}
	}
}