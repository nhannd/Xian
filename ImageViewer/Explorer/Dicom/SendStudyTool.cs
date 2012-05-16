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
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
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

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
    public class SendStudyTool : StudyBrowserTool
    {
		public void SendStudy()
		{
			if (!Enabled)
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
			var code = ApplicationComponent.LaunchAsDialog(Context.DesktopWindow, dialogContainer, SR.TitleSendStudy);

			if (code != ApplicationComponentExitCode.Accepted)
				return;

			if (serverTreeComponent.SelectedServers.Count == 0)
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageSelectDestination, MessageBoxActions.Ok);
				return;
			}

			if (serverTreeComponent.SelectedServers.Count > 1)
			{
				if (Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmSendToMultipleServers, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			try
			{
				if (Context.SelectedStudies.Count > 1)
				{
					var count = ProcessItemsAsync(Context.SelectedStudies, study => SendStudy(study, serverTreeComponent.SelectedServers), false);
					AlertMultipleStudiesSent(count);
				}
				else if (Context.SelectedStudies.Count == 1)
				{
					var study = Context.SelectedStudies.First();
					SendStudy(study, serverTreeComponent.SelectedServers);
					AlertStudySent(study, serverTreeComponent.SelectedServers);
				}
			}
			catch (EndpointNotFoundException)
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageSendDicomServerServiceNotRunning, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageErrorSendingStudies, Context.DesktopWindow);
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
			Enabled = Context.SelectedStudies.Count > 0
			          && Context.SelectedServers.AllSupport<IWorkItemService>()
                      && WorkItemActivityMonitor.IsRunning;
		}

		private void SendStudy(StudyTableItem study, DicomServiceNodeList destinations)
		{
			var client = new DicomSendBridge();
			foreach (var destination in destinations)
			{
				client.MoveStudy(destination, study, WorkItemPriorityEnum.Normal);
			}
		}

		private void AlertMultipleStudiesSent(int count)
		{
			Context.DesktopWindow.ShowAlert(AlertLevel.Info,
											string.Format(SR.MessageFormatSendStudiesScheduled, count),
											SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
		}

		private void AlertStudySent(StudyTableItem study, IEnumerable<IDicomServiceNode> destinations)
		{
			var studyDate = DateParser.Parse(study.StudyDate);
			Context.DesktopWindow.ShowAlert(AlertLevel.Info,
											string.Format(SR.MessageFormatSendStudyScheduled,
											              string.Join(", ", destinations.Select(d => d.Name)),
											              study.PatientsName.FormattedName,
											              studyDate.HasValue
											              	? Format.Date(studyDate.Value)
											              	: string.Empty,
											              study.AccessionNumber),
											SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
		}
	}
}
