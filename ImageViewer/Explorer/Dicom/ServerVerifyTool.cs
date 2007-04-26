using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarVerify")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/MenuVerify")]
    [ClickHandler("activate", "VerifyServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipVerify")]
    [IconSet("activate", IconScheme.Colour, "Icons.Verify.png", "Icons.Verify.png", "Icons.Verify.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerVerifyTool : AENavigatorTool
    {
        public ServerVerifyTool()
        {
        }

        private void VerifyServer()
        {
            if (this.Context.SelectedServers == null || this.Context.SelectedServers.Servers == null || this.Context.SelectedServers.Servers.Count == 0)
            {
				Platform.ShowMessageBox(SR.MessageNoServersSelected, MessageBoxActions.Ok);
            }

            ApplicationEntity myAE = this.Context.ServerTree.RootNode.LocalDataStoreNode.GetApplicationEntity();

            StringBuilder msgText = new StringBuilder();
			msgText.AppendFormat(SR.MessageCEchoVerificationPrefix + "\r\n\r\n");
            using (DicomClient client = new DicomClient(myAE))
            {
				foreach (Server server in this.Context.SelectedServers.Servers)
                {
                    if (client.Verify(server.GetApplicationEntity()))
						msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultSuccess + "\r\n", server.Path);
                    else
						msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultFail + "\r\n", server.Path);
                }
            }
            msgText.AppendFormat("\r\n");
            Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
            return;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            this.Enabled = !this.Context.ServerTree.CurrentNode.Name.Equals(AENavigatorComponent.MyDatastoreTitle)
                            && this.Context.ServerTree.RootNode.ServerGroupNode.ChildServers.Count > 0;
        }
    }
}
