#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarDelete", "DeleteServerServerGroup")]
	[MenuAction("activate", "dicomaenavigator-contextmenu/MenuDelete", "DeleteServerServerGroup")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDelete")]
    [IconSet("activate", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class DeleteServerTool : AENavigatorTool
    {
        public DeleteServerTool()
        {
        }

        private void DeleteServerServerGroup()
        {
            ServerTree serverTree = this.Context.ServerTree;
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
        	//enable only if it's a server or server group, and is not the "My Servers" root node.
			this.Enabled = (this.Context.ServerTree.CurrentNode.IsServer || this.Context.ServerTree.CurrentNode.IsServerGroup) &&
							this.Context.ServerTree.CurrentNode != this.Context.ServerTree.RootNode.ServerGroupNode;
        }
    }
}
