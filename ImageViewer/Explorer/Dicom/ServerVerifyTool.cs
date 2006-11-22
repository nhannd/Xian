using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/Verify")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/Verify (C-ECHO)")]
    [ClickHandler("activate", "VerifyServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "Verify (C-ECHO)")]
    [IconSet("activate", IconScheme.Colour, "Icons.Verify.png", "Icons.Verify.png", "Icons.Verify.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class ServerVerifyTool : AENavigatorTool
    {
        public ServerVerifyTool()
        {
        }

        private void VerifyServer()
        {
            DicomServerTree _dicomServerTree = this.Context.DicomAEServerTree;
            if (!_dicomServerTree.CurrentServer.IsServer && _dicomServerTree.ChildServers.Count == 0)
            {
                throw new DicomServerException("There are no servers selected. Please select servers and try again.");
            }
            LocalAESettings myAESettings = new LocalAESettings();
            ApplicationEntity myAE = new ApplicationEntity(new HostName("localhost"), new AETitle(myAESettings.AETitle), new ListeningPort(myAESettings.Port));
            StringBuilder msgText = new StringBuilder();
            msgText.AppendFormat("C-ECHO Verification:\r\n\r\n");
            using (DicomClient client = new DicomClient(myAE))
            {
                if (_dicomServerTree.CurrentServer.IsServer)
                {
                    DicomServer ae = (DicomServer)_dicomServerTree.CurrentServer;
                    if (client.Verify(ae.DicomAE))
                        msgText.AppendFormat("    {0}: successful    \r\n", ae.ServerPath + "/" + ae.ServerName);
                    else
                        msgText.AppendFormat("    {0}: fail    \r\n", ae.ServerPath + "/" + ae.ServerName);
                }
                else
                {
                    foreach (DicomServer ae in _dicomServerTree.ChildServers)
                    {
                        if (client.Verify(ae.DicomAE))
                            msgText.AppendFormat("    {0}: successful    \r\n", ae.ServerPath + "/" + ae.ServerName);
                        else
                            msgText.AppendFormat("    {0}: fail    \r\n", ae.ServerPath + "/" + ae.ServerName);
                    }
                }
            }
            msgText.AppendFormat("\r\n");
            Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
            return;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            this.Enabled = !this.Context.DicomAEServerTree.CurrentServer.ServerName.Equals(AENavigatorComponent.MyDatastoreTitle)
                            && this.Context.DicomAEServerTree.ChildServers.Count > 0;
        }
    }
}
