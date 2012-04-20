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
using ClearCanvas.Dicom.Iod;
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
            if (!Enabled || Context.SelectedSeries == null)
                return;

            if (StudyInUse())
                return;

            if (!ConfirmDeletion())
                return;

            //TODO (Marmot):Restore.
            try
            {
                var client = new DeleteClient();
                var seriesList = new List<string>();
                foreach (var series in Context.SelectedSeries)
                {
                    seriesList.Add(series.SeriesInstanceUid);
                }

                client.DeleteSeries(Context.Study, seriesList);
                Context.DesktopWindow.ShowAlert(AlertLevel.Info,
                                string.Format(SR.MessageFormatDeleteSeriesScheduled, Context.SelectedSeries.Count, new PersonName(Patient.PatientsName).FormattedName),
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
            var imageViewers = new List<IImageViewer>();

            foreach (Workspace workspace in Context.DesktopWindow.Workspaces)
            {
                IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
                if (viewer == null)
                    continue;

                imageViewers.Add(viewer);
            }

            return imageViewers;
        }

        private void UpdateEnabled()
        {
            Enabled = (Context.SelectedSeries != null &&
                       Context.SelectedSeries.Count > 0 &&
                //TODO (Marmot): This determines local/remote; will be fixing this shortly.
                       Server == null &&
                       WorkItemActivityMonitor.IsRunning);
        }
    }
}