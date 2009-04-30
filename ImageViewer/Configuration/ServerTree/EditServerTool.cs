#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
	[IconSet("activate", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolMedium.png", "Icons.EditToolLarge.png")]
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
			ImageViewer.Services.ServerTree.ServerTree serverTree = this.Context.ServerTree;
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
			//enabled if it is a server or server group and is not the "My Servers" root node
			this.Enabled = (this.Context.ServerTree.CurrentNode.IsServer || this.Context.ServerTree.CurrentNode.IsServerGroup) &&
			               this.Context.ServerTree.CurrentNode != this.Context.ServerTree.RootNode.ServerGroupNode;
		}
	}
}