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

		    var serverTreeComponent = new ServerTreeComponent
		                                  {
		                                      IsReadOnly = true,
		                                      ShowCheckBoxes = false,
		                                      ShowLocalServerNode = false,
		                                      ShowTitlebar = false,
		                                      ShowTools = false
		                                  };

		    var dialogContainer = new SimpleComponentContainer(serverTreeComponent);

			ApplicationComponentExitCode code =
				ApplicationComponent.LaunchAsDialog(
					Context.DesktopWindow,
					dialogContainer,
					SR.TitleSendStudy);

			if (code != ApplicationComponentExitCode.Accepted)
				return;

			if (serverTreeComponent.SelectedServers == null || serverTreeComponent.SelectedServers.Servers == null || serverTreeComponent.SelectedServers.Servers.Count == 0)
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageSelectDestination, MessageBoxActions.Ok);
				return;
			}

			if (serverTreeComponent.SelectedServers.Servers.Count > 1)
			{
				if (Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmSendToMultipleServers, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			var studyUids = new List<string>();
	
            var client = new DicomSendClient();
            foreach (StudyItem item in Context.SelectedStudies)
            {
                studyUids.Add(item.StudyInstanceUid);
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
                        Context.DesktopWindow.ShowAlert(AlertLevel.Info, string.Format(SR.MessageFormatSendStudyScheduled,aeInformation.AETitle),
                                                        SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show);
                    }
                    catch (EndpointNotFoundException)
                    {
                        Context.DesktopWindow.ShowMessageBox(SR.MessageSendDicomServerServiceNotRunning, MessageBoxActions.Ok);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, SR.MessageFailedToSendStudy, Context.DesktopWindow);
                    }
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
