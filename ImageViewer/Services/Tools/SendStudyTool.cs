using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Dicom.Network;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
    [ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarSendStudy")]
    [MenuAction("activate", "dicomstudybrowser-contextmenu/MenuSendStudy")]
    [ClickHandler("activate", "SendStudy")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipSendStudy")]
    [IconSet("activate", IconScheme.Colour, "Icons.SendStudySmall.png", "Icons.SendStudySmall.png", "Icons.SendStudySmall.png")]
    [ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
    public class SendStudyTool : StudyBrowserTool
    {
        public SendStudyTool()
        {

        }

        private void SendStudy()
        {
			BlockingOperation.Run(SendStudyInternal);
        }

		private void SendStudyInternal()
		{
			if (this.Context.SelectedStudy == null)
				return;

			AENavigatorComponent aeNavigator = new AENavigatorComponent(false, false);
			DialogContent content = new DialogContent(aeNavigator);
			DialogComponentContainer dialogContainer = new DialogComponentContainer(content);

			ApplicationComponentExitCode code =
				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					dialogContainer,
					SR.TitleSendStudy);

			if (code == ApplicationComponentExitCode.Cancelled)
				return;

			if (aeNavigator.SelectedServers == null || aeNavigator.SelectedServers.Servers == null || aeNavigator.SelectedServers.Servers.Count == 0)
			{
				Platform.ShowMessageBox(SR.MessageSelectDestination);
				return;
			}

			if (aeNavigator.SelectedServers.Servers.Count > 1)
			{
				if (Platform.ShowMessageBox(SR.MessageConfirmSendToMultipleServers, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			List<string> studyUids = new List<string>();
			foreach (StudyItem item in this.Context.SelectedStudies)
				studyUids.Add(item.StudyInstanceUID);

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				client.Open();

				foreach (Server destinationAE in aeNavigator.SelectedServers.Servers)
				{
					AEInformation aeInformation = new AEInformation();
					aeInformation.AETitle = destinationAE.AETitle;
					aeInformation.HostName = destinationAE.Host;
					aeInformation.Port = destinationAE.Port;
					
					client.Send(aeInformation, studyUids);
				}

				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(this.Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				Platform.ShowMessageBox(SR.MessageSendDicomServerServiceNotRunning);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToSendStudy, this.Context.DesktopWindow);
			}
		}

        protected override void OnSelectedStudyChanged(object sender, EventArgs e)
        {
            // If the results aren't from the local machine, then we don't
            // even care whether a study has been selected or not
            if (!this.Context.SelectedServerGroup.IsLocalDatastore)
                return;

            base.OnSelectedStudyChanged(sender, e);
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            // If no study is selected then we don't even care whether
            // the last searched server has changed.
            if (this.Context.SelectedStudy == null)
                return;

            if (this.Context.SelectedServerGroup.IsLocalDatastore)
                this.Enabled = true;
            else
                this.Enabled = false;
        }
    }
}
