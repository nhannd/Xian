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
	[ButtonAction("activate", "servertree-toolbar/ToolbarEdit", "EditServer")]
	[MenuAction("activate", "servertree-contextmenu/MenuEdit", "EditServer")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipEdit")]
	[IconSet("activate", "Icons.EditToolSmall.png", "Icons.EditToolMedium.png", "Icons.EditToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class EditServerTool : ServerTreeTool
	{
		public EditServerTool()
		{
		}

		public override void Initialize()
		{
			SetDoubleClickHandler();

			base.Initialize();
		}

		private void EditServer()
		{
			Common.ServerTree.ServerTree serverTree = this.Context.ServerTree;
			this.Context.UpdateType = (int)ServerUpdateType.Edit;

			if (serverTree.CurrentNode.IsServer)
			{
				DicomServerEditComponent editor = new DicomServerEditComponent(serverTree);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleEditServer);
			}
			else
			{
				DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(serverTree, ServerUpdateType.Edit);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleEditServerGroup);
			}

			this.Context.UpdateType = (int)ServerUpdateType.None;
		}

		private void SetDoubleClickHandler()
		{
			this.Context.DefaultActionHandler = EditServer;
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			if (this.Context.IsReadOnly)
			{
				this.Enabled = false;
			}
			else
			{
				//enabled if it is a server or server group and is not the "My Servers" root node
				this.Enabled = (this.Context.ServerTree.CurrentNode.IsServer || this.Context.ServerTree.CurrentNode.IsServerGroup) &&
				               this.Context.ServerTree.CurrentNode != this.Context.ServerTree.RootNode.ServerGroupNode;
			}
		}
	}
}