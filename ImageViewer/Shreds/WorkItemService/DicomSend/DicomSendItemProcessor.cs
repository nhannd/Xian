#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomSend
{
    /// <summary>
    ///  Class for processing DICOM Sends.
    /// </summary>
    internal class DicomSendItemProcessor : BaseItemProcessor<DicomSendRequest, DicomSendProgress>
    {
        #region Private Members

        private ImageViewerStorageScu _scu;

        #endregion

        #region Public Properties

        public DicomSendStudyRequest SendStudy
        {
            get { return Request as DicomSendStudyRequest; }
        }

        public DicomSendSeriesRequest SendSeries
        {
            get { return Request as DicomSendSeriesRequest; }
        }

        public DicomSendSopRequest SendSops
        {
            get { return Request as DicomSendSopRequest; }
        }

        public DicomAutoRouteRequest AutoRoute
        {
            get { return Request as DicomAutoRouteRequest; }
        }

        public PublishFilesRequest PublishFiles
        {
            get { return Request as PublishFilesRequest; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cleanup any failed items in the queue and delete the queue entry.
        /// </summary>
        public override void Delete()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemUidBroker();
                var uidBroker = context.GetWorkItemUidBroker();

                var list = broker.GetWorkItemUidsForWorkItem(Proxy.Item.Oid);
                foreach (WorkItemUid sop in list)
                {
                    uidBroker.Delete(sop);
                }
                context.Commit();
            }

            if (PublishFiles != null && PublishFiles.DeletionBehaviour == DeletionBehaviour.DeleteAlways)
            {
                foreach (string file in PublishFiles.FilePaths)
                {
                    try
                    {
                        FileUtils.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Warn, "Unable to delete temporary publish file {0}: {1}", file, e.Message);
                    }
                }
            }

            Proxy.Delete();
        }

        /// <summary>
        /// Override of Cancel() routine.
        /// </summary>
        /// <remarks>
        /// The Cancel must be overriden to call the SCU's Cancel routine.
        /// </remarks>
        public override void Cancel()
        {
            _scu.Cancel();
            base.Cancel();
        }

        /// <summary>
        /// Override of Stop() routine.
        /// </summary>
        /// <remarks>
        /// The Stop must be override to call the SCU's Cancel routine.
        /// </remarks>
        public override void Stop()
        {
            _scu.Cancel();
            base.Stop();
        }

        public override void Process()
        {
            DicomServerConfiguration configuration = GetServerConfiguration();
            var remoteAE = ServerDirectory.GetRemoteServerByName(Request.Destination);
            if (remoteAE == null)
            {
                Proxy.Fail(string.Format("Unknown destination: {0}",Request.Destination),WorkItemFailureType.Fatal);
                return;

            }

            if (AutoRoute != null)
            {
                DateTime now = Platform.Time;
                DateTime scheduledTime = AutoRoute.GetScheduledTime(now, 0);
                if (now != scheduledTime)
                {
                    Platform.Log(LogLevel.Info, "Rescheduling AutoRoute WorkItem {0} back into the scheduled time window: {1}", Proxy.Item.Oid, scheduledTime);
                    Proxy.Postpone(scheduledTime);
                    return;
                }
            }

            _scu = new ImageViewerStorageScu(configuration.AETitle, remoteAE);
            
            LoadImagesToSend();

            if (Request.CompressionType != CompressionType.None)
            {
                _scu.LoadPreferredSyntaxes(Request);    
            }

            Progress.ImagesToSend = _scu.TotalSubOperations;            
            Progress.FailureSubOperations = 0;
            Progress.WarningSubOperations = 0;
            Progress.SuccessSubOperations = 0;
            Progress.IsCancelable = true;
            Proxy.UpdateProgress();

            _scu.ImageStoreCompleted += OnImageSent;

            _scu.DoSend();

            if (_scu.Canceled)
            {
                if (StopPending)
                {
                    Proxy.Postpone();
                }
                else
                {
                    Proxy.Cancel();
                }
            } 
            else if (_scu.Failed || _scu.FailureSubOperations > 0)
            {
                if (AutoRoute != null)
                {
                    Proxy.Fail(_scu.FailureDescription, WorkItemFailureType.NonFatal, AutoRoute.GetScheduledTime(Platform.Time, (int)WorkItemServiceSettings.Instance.PostponeSeconds));
                }
                else
                    Proxy.Fail(_scu.FailureDescription,WorkItemFailureType.NonFatal);
            }
            else
            {
                Proxy.Complete();
            }
        }

        #endregion

        #region Private Methods

        private void LoadImagesToSend()
        {
            if (SendStudy != null)
            {
                var studyXml = Location.LoadStudyXml();

                _scu.LoadStudyFromStudyXml(Location, studyXml);
            }
            else if (SendSeries != null)
            {
                var studyXml = Location.LoadStudyXml();

                foreach (var seriesInstanceUid in SendSeries.SeriesInstanceUids)
                {
                    _scu.LoadSeriesFromStudyXml(Location, studyXml, seriesInstanceUid);
                }
            }
            else if (SendSops != null)
            {
                foreach (string sop in SendSops.SopInstanceUids)
                {
                    _scu.AddFile(Location.GetSopInstancePath(SendSops.SeriesInstanceUid, sop));
                }
            }
            else if (AutoRoute != null)
            {
                LoadUids();
                if (WorkQueueUidList.Count == 0)
                {
                    var studyXml = Location.LoadStudyXml();

                    _scu.LoadStudyFromStudyXml(Location, studyXml);
                }
                else
                {
                    foreach (var uid in WorkQueueUidList)
                    {
                        _scu.AddFile(Location.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid));
                    }
                }
            }
            else if (PublishFiles != null)
            {
                foreach (var path in PublishFiles.FilePaths)
                    _scu.AddFile(path);
            }
        }

        private void OnImageSent(object sender, StorageInstance storageInstance)
        {
            Progress.ImagesToSend = _scu.TotalSubOperations;

            if (storageInstance.SendStatus.Status == DicomState.Success)
            {
                Progress.SuccessSubOperations++;
                Progress.StatusDetails = string.Empty;
            }
            else if (storageInstance.SendStatus.Status == DicomState.Failure)
            {
                Progress.FailureSubOperations++;
                Progress.StatusDetails = storageInstance.ExtendedFailureDescription;
                if (String.IsNullOrEmpty(Progress.StatusDetails))
                    Progress.StatusDetails = storageInstance.SendStatus.ToString();
            }
            else if (storageInstance.SendStatus.Status == DicomState.Warning)
            {
                Progress.WarningSubOperations++;
                Progress.StatusDetails = storageInstance.ExtendedFailureDescription;
                if (String.IsNullOrEmpty(Progress.StatusDetails))
                    Progress.StatusDetails = storageInstance.SendStatus.ToString();
            }

            Proxy.UpdateProgress();

            if (PublishFiles != null &&
                PublishFiles.DeletionBehaviour != DeletionBehaviour.None)
            {
                bool deleteFile = false;
                if (storageInstance.SendStatus.Status != DicomState.Failure)
                    deleteFile = true;
                else if (PublishFiles.DeletionBehaviour == DeletionBehaviour.DeleteAlways)
                    deleteFile = true;

                if (deleteFile)
                {
                    try
                    {
                        FileUtils.Delete(storageInstance.Filename);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Warn, e,
                                     "Failed to delete file after storage: {0}",
                                     storageInstance.Filename);
                    }
                }
            }
        }

        public override bool CanStart(out string reason)
        {
            var relatedList = FindRelatedWorkItems(null, new List<WorkItemStatusEnum> { WorkItemStatusEnum.InProgress });
            if (relatedList.Count > 0)
            {
                reason = "There are related WorkItems for the study being processed.";
                return false;
            }
            
            // Pending, InProgress, Idle ProcessStudy entries existing.
            relatedList = FindRelatedWorkItems(new List<string>{ ProcessStudyRequest.WorkItemTypeString }, new List<WorkItemStatusEnum> { WorkItemStatusEnum.InProgress, WorkItemStatusEnum.Idle, WorkItemStatusEnum.Pending });

            if (relatedList.Count > 0)
            {
                reason = "There are related WorkItems for the study being processed.";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        #endregion
    }
}
