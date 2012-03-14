#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarSendStudy", "SendStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuSendStudy", "SendStudy")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipSendStudy")]
	[IconSet("activate", "Icons.SendStudyToolSmall.png", "Icons.SendStudyToolSmall.png", "Icons.SendStudyToolSmall.png")]

	[ViewerActionPermission("activate", ImageViewer.Services.AuthorityTokens.Study.Send)]

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
			if (!Enabled || this.Context.SelectedStudy == null)
				return;

			ServerTreeComponent serverTreeComponent = new ServerTreeComponent();
			serverTreeComponent.IsReadOnly = true;
			serverTreeComponent.ShowCheckBoxes = false;
			serverTreeComponent.ShowLocalDataStoreNode = false;
			serverTreeComponent.ShowTitlebar = false;
			serverTreeComponent.ShowTools = false;

			SimpleComponentContainer dialogContainer = new SimpleComponentContainer(serverTreeComponent);

			ApplicationComponentExitCode code =
				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					dialogContainer,
					SR.TitleSendStudy);

			if (code != ApplicationComponentExitCode.Accepted)
				return;

			if (serverTreeComponent.SelectedServers == null || serverTreeComponent.SelectedServers.Servers == null || serverTreeComponent.SelectedServers.Servers.Count == 0)
			{
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageSelectDestination, MessageBoxActions.Ok);
				return;
			}

			if (serverTreeComponent.SelectedServers.Servers.Count > 1)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmSendToMultipleServers, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			EventResult result = EventResult.Success;
			AuditedInstances sentInstances = new AuditedInstances();

			List<string> studyUids = new List<string>();
			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				studyUids.Add(item.StudyInstanceUid);
				sentInstances.AddInstance(item.PatientId, item.PatientsName, item.StudyInstanceUid);
			}

			DicomSendServiceClient client = new DicomSendServiceClient();

			try
			{
				client.Open();

				foreach (Server destinationAE in serverTreeComponent.SelectedServers.Servers)
				{
					SendStudiesRequest request = new SendStudiesRequest();
					AEInformation aeInformation = new AEInformation();
					aeInformation.AETitle = destinationAE.AETitle;
					aeInformation.HostName = destinationAE.Host;
					aeInformation.Port = destinationAE.Port;
					request.DestinationAEInformation = aeInformation;
					request.StudyInstanceUids = studyUids;
					client.SendStudies(request);
				}

				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(this.Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				result = EventResult.SeriousFailure;
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageSendDicomServerServiceNotRunning, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				result = EventResult.MajorFailure;
				ExceptionHandler.Report(e, SR.MessageFailedToSendStudy, this.Context.DesktopWindow);
			}
			finally
			{
				foreach (Server destinationAE in serverTreeComponent.SelectedServers.Servers)
					AuditHelper.LogBeginSendInstances(destinationAE.AETitle, destinationAE.Host, sentInstances, EventSource.CurrentUser, result);
			}
		}

        protected override void OnSelectedStudyChanged(object sender, EventArgs e)
        {
        	UpdateEnabled();
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			Enabled = (this.Context.SelectedStudy != null &&
			           this.Context.SelectedServerGroup.IsLocalDatastore &&
			           LocalDataStoreActivityMonitor.IsConnected);
		}
	}
}
