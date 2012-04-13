#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomSend
{
    public class DicomSendItemProcessor : BaseItemProcessor<DicomSendRequest, DicomSendProgress>
    {
        private ImageViewerStorageScu _scu;

        public DicomSendStudyRequest SendStudy
        {
            get { return Request as DicomSendStudyRequest; }
        }

        public DicomSendSeriesRequest SendSeries
        {
            get { return Request as DicomSendSeriesRequest; }
        }

        public DicomAutoRouteRequest AutoRoute
        {
            get { return Request as DicomAutoRouteRequest; }
        }

        public PublishFilesRequest PublishFiles
        {
            get { return Request as PublishFilesRequest; }
        }

        public override void Cancel()
        {
            _scu.Cancel();
            base.Cancel();
        }

        public override void Stop()
        {
            _scu.Cancel();
            base.Stop();
        }

        public override void Process()
        {

            _scu = new ImageViewerStorageScu("", Request);

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
            else if (AutoRoute != null)
            {
                LoadUids();
                foreach (WorkItemUid uid in WorkQueueUidList)
                {
                    _scu.AddFile(Location.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid));
                }
            }
            else if (PublishFiles != null)
            {
                foreach (var path in PublishFiles.FilePaths)
                    _scu.AddFile(path);
            }

            

            _scu.ImageStoreCompleted += delegate(object sender, StorageInstance storageInstance)
                                           {
                                               Progress.ImagesToSend = _scu.TotalSubOperations;

                                               if (storageInstance.SendStatus.Status == DicomState.Success)
                                               {
                                                   Progress.ImagesSuccess++;
                                                   Progress.StatusDetails = string.Empty;
                                               }
                                               else if (storageInstance.SendStatus.Status == DicomState.Failure)
                                               {
                                                   Progress.ImagesError++;
                                                   Progress.StatusDetails = storageInstance.ExtendedFailureDescription;
                                                   if (String.IsNullOrEmpty(Progress.StatusDetails))
                                                       Progress.StatusDetails = storageInstance.SendStatus.ToString();
                                               }
                                               else if (storageInstance.SendStatus.Status == DicomState.Warning)
                                               {
                                                   Progress.ImagesWarning++;
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
                                                   else if (PublishFiles.DeletionBehaviour ==
                                                            DeletionBehaviour.DeleteAlways)
                                                       deleteFile = true;

                                                   if (deleteFile)
                                                   {
                                                       try
                                                       {
                                                           File.Delete(storageInstance.Filename);
                                                       }
                                                       catch (Exception e)
                                                       {
                                                           Platform.Log(LogLevel.Warn, e,
                                                                        "Failed to delete file after storage: {0}",
                                                                        storageInstance.Filename);
                                                       }
                                                   }
                                               }
                                           };

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
            else if (_scu.Failed)
            {
                Proxy.Fail(_scu.FailureDescription,WorkItemFailureType.NonFatal);
            }
        }
    }
}
