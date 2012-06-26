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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
    [ButtonAction("activate", ToolbarActionSite + "/ToolbarSendSeries", "SendSeries")]
    [MenuAction("activate", ContextMenuActionSite + "/MenuSendSeries", "SendSeries")]
    [Tooltip("activate", "TooltipSendSeries")]
    [IconSet("activate", "Icons.SendSeriesToolSmall.png", "Icons.SendSeriesToolSmall.png", "Icons.SendSeriesToolSmall.png")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ViewerActionPermission("activate", Common.AuthorityTokens.Study.Send)]
    [ExtensionOf(typeof(SeriesDetailsToolExtensionPoint))]
    public class SendSeriesTool : SeriesDetailsTool
    {
        public void SendSeries()
        {
            BlockingOperation.Run(SendSeriesInternal);
        }

        private void SendSeriesInternal()
        {
            if (!Enabled || this.Context.SelectedSeries == null)
                return;

            if (SelectedSeries.Any(series => series.ScheduledDeleteTime.HasValue))
            {
                Context.DesktopWindow.ShowMessageBox(SR.MessageCannotSendSeriesScheduledForDeletion, MessageBoxActions.Ok);
                return;
            }

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
                    SR.TitleSendSeries);

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

            var client = new DicomSendBridge();
            var seriesUids = Context.SelectedSeries.Select(item => item.SeriesInstanceUid).ToList();

            foreach (var destination in serverTreeComponent.SelectedServers)
            {
                try
                {
                    client.SendSeries(destination, Context.Study, seriesUids.ToArray(), WorkItemPriorityEnum.Normal);
                    DateTime? studyDate = DateParser.Parse(Context.Study.StudyDate);
                    Context.DesktopWindow.ShowAlert(AlertLevel.Info,
                                                    string.Format(SR.MessageFormatSendSeriesScheduled, seriesUids.Count,
                                                                  destination.Name, Context.Study.PatientsName.FormattedName, studyDate.HasValue ? Format.Date(studyDate.Value) : string.Empty,
                                                                  Context.Study.AccessionNumber),
                                                    SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
                }
                catch (EndpointNotFoundException)
                {
                    Context.DesktopWindow.ShowMessageBox(SR.MessageSendDicomServerServiceNotRunning,
                                                         MessageBoxActions.Ok);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.MessageFailedToSendSeries, Context.DesktopWindow);
                }
            }
        }

        protected override void OnSelectedSeriesChanged()
        {
            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            Enabled = Context.SelectedSeries.Count > 0
                        && Server.IsSupported<IWorkItemService>()
                        && WorkItemActivityMonitor.IsRunning;
        }
    }
}