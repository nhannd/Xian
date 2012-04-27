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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
    [ButtonAction("activate", ToolbarActionSite + "/ToolbarDeleteSeries", "DeleteSeries")]
    [MenuAction("activate", ContextMenuActionSite + "/MenuDeleteSeries", "DeleteSeries")]
    [Tooltip("activate", "TooltipDeleteSeries")]
    [IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ViewerActionPermission("activate", Common.AuthorityTokens.Study.Delete)]
    [ExtensionOf(typeof(SeriesDetailsToolExtensionPoint))]
    public class DeleteSeriesTool : SeriesDetailsTool
    {
        public void DeleteSeries()
        {
            if (!Enabled)
                return;

            if (StudyInUse())
                return;

            if (!ConfirmDeletion())
                return;

            try
            {
                var client = new DeleteClient();
                var seriesList = Context.SelectedSeries.Select(series => series.SeriesInstanceUid).ToList();
                client.DeleteSeries(Context.Study, seriesList);
                
                Context.RefreshSeriesTable();

                DateTime? studyDate = DateParser.Parse(Context.Study.StudyDate);
                Context.DesktopWindow.ShowAlert(AlertLevel.Info,
                                String.Format(SR.MessageFormatDeleteSeriesScheduled, Context.SelectedSeries.Count, Context.Study.PatientsName.FormattedName, studyDate.HasValue ? Format.Date(studyDate.Value) : string.Empty,
                                                                          Context.Study.AccessionNumber),
                                SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show);

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.MessageFailedToDeleteSeries, Context.DesktopWindow);
            }
        }

        protected override void OnSelectedSeriesChanged()
        {
            UpdateEnabled();
        }

        private bool ConfirmDeletion()
        {
            string message = Context.SelectedSeries.Count == 1
                                 ? SR.MessageConfirmDeleteSeries
                                 : String.Format(SR.MessageFormatConfirmDeleteSeries, Context.SelectedSeries.Count);

            DialogBoxAction action = Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);

            if (action == DialogBoxAction.Yes)
                return true;
            return false;
        }

        // This is a total hack to prevent a user from deleting a study
        // that is currently in use.  The proper way of doing this is
        // to lock the study when it's in use.  But for now, this will do.
        private bool StudyInUse()
        {
            IEnumerable<IImageViewer> imageViewers = GetImageViewers();

            foreach (IImageViewer imageViewer in imageViewers)
            {
                if (imageViewer.StudyTree.GetStudy(Context.Study.StudyInstanceUid) != null)
                {
                    string message = SR.MessageStudyInUse;
                    Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<IImageViewer> GetImageViewers()
        {
            return Context.DesktopWindow.Workspaces
                .Select(ImageViewerComponent.GetAsImageViewer).Where(viewer => viewer != null);
        }

        private void UpdateEnabled()
        {
            Enabled = Context.SelectedSeries.Count > 0 
                        && Server.IsSupported<IWorkItemService>()
                        && WorkItemActivityMonitor.IsRunning;
        }
    }
}