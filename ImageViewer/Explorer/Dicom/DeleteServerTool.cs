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
                if (Platform.ShowMessageBox(SR.MessageConfirmDeleteServer, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
                    return;

                this.Context.UpdateType = (int)ServerUpdateType.Delete;
                serverTree.DeleteDicomServer();
				this.Context.UpdateType = (int)ServerUpdateType.None; 
            }
            else if (serverTree.CurrentNode.IsServerGroup)
            {
                if (Platform.ShowMessageBox(SR.MessageConfirmDeleteServerGroup, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
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
