using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarDelete")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/MenuDelete")]
    [ClickHandler("activate", "DeleteServerServerGroup")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDelete")]
    [IconSet("activate", IconScheme.Colour, "Icons.Delete.png", "Icons.Delete.png", "Icons.Delete.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerDeleteTool : AENavigatorTool
    {
        public ServerDeleteTool()
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
            }
            else
            {
                if (Platform.ShowMessageBox(SR.MessageConfirmDeleteServerGroup, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
                    return;

                this.Context.UpdateType = (int)ServerUpdateType.Delete;
                serverTree.DeleteServerGroup();
            }
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            this.Enabled = !this.Context.ServerTree.CurrentNode.Name.Equals(AENavigatorComponent.MyDatastoreTitle)
                            && !this.Context.ServerTree.CurrentNode.Name.Equals(AENavigatorComponent.MyServersTitle);
        }
    }
}
