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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.StudyProcess
{
    public class StudyProcessProcessor : BaseItemProcessor
    {
        public StudyProcessProgress Progress
        {
            get { return Proxy.Item.Progress as StudyProcessProgress; }
        }

        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            if (proxy.Item.Progress == null)
                proxy.Item.Progress = new StudyProcessProgress();
            else if (!(proxy.Item.Progress is StudyProcessProgress))
                proxy.Item.Progress = new StudyProcessProgress();

            return base.Initialize(proxy);
        }
        /// <summary>
        /// Cleanup any failed items in the queue and delete the queue entry.
        /// </summary>
        /// <param name="proxy"></param>
        public override void Delete(WorkItemStatusProxy proxy)
        {
            LoadUids();
            foreach (WorkItemUid sop in WorkQueueUidList)
            {
                if (sop.Failed)
                {
                    try
                    {
                        string file = !string.IsNullOrEmpty(sop.File)
                                          ? Location.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid)
                                          : Path.Combine(Location.StudyFolder, sop.File);
                        FileUtils.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception attempting to cleanup file for Work Item {0}",
                                     proxy.Item.Oid);
                    }
                }
            }

            Proxy.Delete();
        }

        public override void Process(WorkItemStatusProxy proxy)
        {
            int count = ProcessUidList();

            if (CancelPending)
            {
                Proxy.Cancel();
                return;                
            }

            if (StopPending)
            {
                Proxy.Postpone();
                return;
            }

            if (count == 0)
            {
                bool failed = false;
                bool complete = true;
                foreach (WorkItemUid sop in WorkQueueUidList)
                {
                    if (sop.Failed)
                    {
                        failed = true;
                    }
                    if (!sop.Complete)
                        complete = false;
                }

                DateTime now = Platform.Time;

                if (failed)
                    Proxy.Fail("",WorkItemFailureType.NonFatal);
                else if (!complete)
                    Proxy.Postpone();
                else if (now > Proxy.Item.ExpirationTime)
                {
                    // Need to apply Study Level Rules here!

                    Proxy.Complete();
                }
                else
                {
                    Proxy.Idle();
                }
            }
            else
            {
                Proxy.Postpone();
            }
        }

        protected override bool CanStart()
        {
            var relatedList = FindRelatedWorkItems(null, new List<WorkItemStatusEnum> {WorkItemStatusEnum.InProgress});

            if (relatedList.Count > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Process all of the SOP Instances associated with a <see cref="WorkItem"/> item.
        /// </summary>
        /// <returns>Number of instances that have been processed successfully.</returns>
        private int ProcessUidList()
        {
            StudyXml studyXml = LoadStudyXml();
            
            int successfulProcessCount = 0;
            int lastSuccessProcessCount = -1;


            // Loop through requerying the database
            while (successfulProcessCount > lastSuccessProcessCount)
            {
                lastSuccessProcessCount = successfulProcessCount;

                LoadUids();

                Progress.TotalFilesToProcess = WorkQueueUidList.Count;
                Progress.UpdateStatus();
                Proxy.UpdateProgress();                        

                foreach (WorkItemUid sop in WorkQueueUidList)
                {
                    if (sop.Failed)
                        continue;
                    if (sop.Complete)
                        continue;

                    if (CancelPending)
                    {
                        Platform.Log(LogLevel.Info, "Processing of study canceled: {0}",
                                     Location.Study.StudyInstanceUid);
                        return successfulProcessCount;
                    }

                    if (StopPending)
                    {
                        Platform.Log(LogLevel.Info, "Processing of study stopped: {0}",
                                     Location.Study.StudyInstanceUid);
                        return successfulProcessCount;
                    }

                    if (ProcessWorkQueueUid(sop, studyXml))
                    {
                        successfulProcessCount++;

                        Progress.NumberOfFilesProcessed++;
                        Progress.UpdateStatus();
                        Proxy.UpdateProgress();
                    }
                    else if (sop.Failed)
                    {
                        Progress.NumberOfProcessingFailures++;
                        Progress.UpdateStatus();
                        Proxy.UpdateProgress();                        
                    }
                }                
            }

            return successfulProcessCount;
        }

        /// <summary>
        /// Process a specified <see cref="WorkItemUid"/>
        /// </summary>
        /// <param name="sop">The <see cref="WorkItemUid"/> being processed</param>
        /// <param name="studyXml">The <see cref="StudyXml"/> object for the study being processed</param>
        /// <returns>true if the <see cref="WorkItemUid"/> is successfully processed. false otherwise</returns>
        protected virtual bool ProcessWorkQueueUid(WorkItemUid sop, StudyXml studyXml)
        {
            Platform.CheckForNullReference(sop, "sop");
            Platform.CheckForNullReference(studyXml, "studyXml");

            string path = null;

            try
            {
                DicomFile file;

                if (!string.IsNullOrEmpty(sop.File))
                {
                    path = Path.Combine(Location.StudyFolder, sop.File);
                    file = new DicomFile(path);

                    // Load the entire file, since it will be re-written over the top
                    file.Load();
                }
                else
                {
                    path = Location.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid);
                    file = new DicomFile(path);

                    // WARNING:  If we ever do anything where we update files and save them,
                    // we may have to change this.
                    file.Load(DicomReadOptions.StorePixelDataReferences);
                }

				var processor = new ClearCanvas.ImageViewer.Dicom.Core.SopInstanceProcessor(Location);

                processor.ProcessFile(file, studyXml, sop);

                return true;
            }
            catch (Exception e)
            {
                var updatedSop = FailWorkItemUid(sop, true);
                sop.Failed = updatedSop.Failed;
                sop.FailureCount = updatedSop.FailureCount;

                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path ?? string.Empty,
                             sop.SopInstanceUid);
                Proxy.Item.Progress.Status = e.InnerException != null
                                                            ? String.Format("{0}:{1}", e.GetType().Name,
                                                                            e.InnerException.Message)
                                                            : String.Format("{0}:{1}", e.GetType().Name, e.Message);
                return false;
            }
        }
    }
}
