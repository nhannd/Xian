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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarSendStudy", "SendStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuSendStudy", "SendStudy")]
    [ActionFormerly("activate", "ClearCanvas.ImageViewer.Services.Tools.SendStudyTool:activate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipSendStudy")]
	[IconSet("activate", "Icons.SendStudyToolSmall.png", "Icons.SendStudyToolSmall.png", "Icons.SendStudyToolSmall.png")]

    [ViewerActionPermission("activate", ImageViewer.Common.AuthorityTokens.Study.Send)]

	//TODO (Marmot):Restore.

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
    public class SendStudyTool : StudyBrowserTool
    {
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
			serverTreeComponent.ShowLocalServerNode = false;
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
	
		    //TODO (Marmot):Restore.
            var client = new DicomSendClient();
            foreach (StudyItem item in this.Context.SelectedStudies)
            {
                studyUids.Add(item.StudyInstanceUid);
                sentInstances.AddInstance(item.PatientId, item.PatientsName, item.StudyInstanceUid);
                foreach (IServerTreeDicomServer destination in serverTreeComponent.SelectedServers.Servers)
                {
                    var aeInformation = new ApplicationEntity
                    {
                        AETitle = destination.AETitle,
                        ScpParameters = new ScpParameters(destination.HostName, destination.Port)
                    };

                    try
                    {
                        client.MoveStudy(aeInformation, item, WorkItemPriorityEnum.Normal);
                    }
                    catch (EndpointNotFoundException)
                    {
                        result = EventResult.SeriousFailure;
                        this.Context.DesktopWindow.ShowMessageBox(SR.MessageSendDicomServerServiceNotRunning, MessageBoxActions.Ok);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, SR.MessageFailedToSendStudy, this.Context.DesktopWindow);
                    }

                    foreach (IServerTreeDicomServer destinationAE in serverTreeComponent.SelectedServers.Servers)
                        AuditHelper.LogBeginSendInstances(destinationAE.AETitle, destinationAE.HostName, sentInstances, EventSource.CurrentUser, result);
                }
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
			           this.Context.SelectedServerGroup.IsLocalServer &&
			           WorkItemActivityMonitor.IsRunning);
		}
	}
}
