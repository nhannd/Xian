using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/Delete")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/Delete")]
    [ClickHandler("activate", "DeleteServerServerGroup")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "Delete")]
    [IconSet("activate", IconScheme.Colour, "Icons.Delete.png", "Icons.Delete.png", "Icons.Delete.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerDeleteTool : AENavigatorTool
    {
        public ServerDeleteTool()
        {
        }

        private void DeleteServerServerGroup()
        {
            DicomServerTree _dicomServerTree = this.Context.DicomAEServerTree;
            string msg = "";
            if (_dicomServerTree.CurrentServer.IsServer)
				msg = SR.MessageConfirmDeleteServer;
            else
				msg = SR.MessageConfirmDeleteServerGroup;
            if (Platform.ShowMessageBox(msg, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
                return;

            this.Context.UpdateType = (int)ServerUpdateType.Delete;
            _dicomServerTree.DeleteDicomServer();
            return;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            this.Enabled = !this.Context.DicomAEServerTree.CurrentServer.ServerName.Equals(AENavigatorComponent.MyDatastoreTitle)
                            && !this.Context.DicomAEServerTree.CurrentServer.ServerName.Equals(AENavigatorComponent.MyServersTitle);
        }
    }
}
