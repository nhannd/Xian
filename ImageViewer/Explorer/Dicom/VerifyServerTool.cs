using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ButtonAction("activate", "dicomaenavigator-toolbar/ToolbarVerify")]
    [MenuAction("activate", "dicomaenavigator-contextmenu/MenuVerify")]
    [ClickHandler("activate", "VerifyServer")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipVerify")]
	[IconSet("activate", IconScheme.Colour, "Icons.VerifyServerToolSmall.png", "Icons.VerifyServerToolMedium.png", "Icons.VerifyServerToolLarge.png")]
    [ExtensionOf(typeof(AENavigatorToolExtensionPoint))]
    public class VerifyServerTool : AENavigatorTool
    {
        public VerifyServerTool()
		{
		}

		private bool NoServersSelected()
		{
			return this.Context.SelectedServers == null || this.Context.SelectedServers.Servers == null || this.Context.SelectedServers.Servers.Count == 0;
		}

		private void VerifyServer()
		{
			BlockingOperation.Run(this.InternalVerifyServer);
		}

		private void InternalVerifyServer()
		{
			if (this.NoServersSelected())
			{
				//should never get here because the verify button should be disabled.
				Platform.ShowMessageBox(SR.MessageNoServersSelected, MessageBoxActions.Ok);
				return;
			}

			ApplicationEntity myAE;
			try
			{
				myAE = new ApplicationEntity(new HostName(DicomServerConfigurationHelper.Host),
												new AETitle(DicomServerConfigurationHelper.AETitle),
												new ListeningPort(DicomServerConfigurationHelper.Port));
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
				return;
			}

			StringBuilder msgText = new StringBuilder();
			msgText.AppendFormat(SR.MessageCEchoVerificationPrefix + "\r\n\r\n");
			using (DicomClient client = new DicomClient(myAE))
			{
				foreach (Server server in this.Context.SelectedServers.Servers)
				{
					bool succeeded = false;
					try
					{
						succeeded = client.Verify(new ApplicationEntity(new HostName(server.Host), new AETitle(server.AETitle), new ListeningPort(server.Port)));
					}
					catch (Exception e)
					{
						Platform.Log(e);
					}
					finally
					{
						if (succeeded)
							msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultSuccess + "\r\n", server.Path);
						else
							msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultFail + "\r\n", server.Path);
					}
				}
			}
			msgText.AppendFormat("\r\n");
			Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
		}

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
			this.Enabled = !this.Context.SelectedServers.IsLocalDatastore && !this.NoServersSelected();                            
        }
    }
}
