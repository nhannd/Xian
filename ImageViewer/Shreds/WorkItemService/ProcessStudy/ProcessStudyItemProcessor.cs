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
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.ProcessStudy
{
    /// <summary>
    /// Processor for <see cref="ProcessStudyRequest"/> entries.
    /// </summary>
    internal class StudyProcessProcessor : BaseItemProcessor<ProcessStudyRequest,ProcessStudyProgress>
    {
        // TODO (CR Jun 2012 - Med): I think this class should be doing some auditing.

        #region Public Properties

        public Study Study { get; set; }

        #endregion

        /// <summary>
        /// Cleanup any failed items in the queue and delete the queue entry.
        /// </summary>
        public override void Delete()
        {
            LoadUids();
            var studyXml = Location.LoadStudyXml();

            foreach (WorkItemUid sop in WorkQueueUidList)
            {
                if (sop.Failed || !sop.Complete)
                {
                    if (!string.IsNullOrEmpty(sop.File))
                    {
                        try
                        {
                            FileUtils.Delete(Path.Combine(Location.StudyFolder, sop.File));
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, e,
                                         "Unexpected exception attempting to cleanup file for Work Item {0}",
                                         Proxy.Item.Oid);
                        }
                    }
                    else
                    {
                        try
                        {
                            // Only delete the file if its not in the study Xml file.  This should handle collisions with 
                            // multiple WorkItems that may have been canceled when others succeeded.
                            if (!studyXml.Contains(sop.SeriesInstanceUid, sop.SopInstanceUid))
                            {
                                string file = Location.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid);
                                FileUtils.Delete(file);
                            }
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, e,
                                         "Unexpected exception attempting to cleanup file for Work Item {0}",
                                         Proxy.Item.Oid);
                        }
                    }
                }
            }

            try
            {
                DirectoryUtility.DeleteIfEmpty(Location.StudyFolder);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception attempting to delete folder: {0}",
                             Location.StudyFolder);
            }            

            // Now cleanup the actual WorkItemUid references
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

            Proxy.Delete();
        }

        /// <summary>
        /// Main Processing routine.
        /// </summary>
        public override void Process()
        {
            int count = ProcessUidList();

            if (CancelPending)
            {
                Proxy.Cancel();
                return;                
            }

            if (StopPending)
            {
                Proxy.Idle();
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
                        break;
                    }
                    else if (!sop.Complete)
                    {
                        complete = false;
                        break;
                    }
                }

                DateTime now = Platform.Time;

                if (failed)
                    Proxy.Fail(WorkItemFailureType.NonFatal);
                else if (!complete)
                    Proxy.Idle();
                else if (now > Proxy.Item.ExpirationTime)
                {
                    if (Study == null)
                        Study = LoadRelatedStudy();

                    var ruleOptions = new RulesEngineOptions
                                          {
                                              ApplyDeleteActions = true,
                                              ApplyRouteActions = true
                                          };
					RulesEngine.Create().ApplyStudyRules(Study.ToStoreEntry(), ruleOptions);

                    Proxy.Complete();
                }
                else
                {
                    Proxy.Idle();
                }
            }
            else
            {
                Proxy.Idle();
            }
        }

        /// <summary>
        /// Process all of the SOP Instances associated with a <see cref="WorkItem"/> item.
        /// </summary>
        /// <returns>Number of instances that have been processed successfully.</returns>
        private int ProcessUidList()
        {
            StudyXml studyXml = Location.LoadStudyXml();
            
            int successfulProcessCount = 0;
            int lastSuccessProcessCount = -1;


            // Loop through requerying the database
            while (successfulProcessCount > lastSuccessProcessCount)
            {
                // If we're just doing a few at a time, less than the batch size, Postpone for now
                if (lastSuccessProcessCount != -1 && (successfulProcessCount - lastSuccessProcessCount) < WorkItemServiceSettings.Default.StudyProcessBatchSize)
                    break;

                lastSuccessProcessCount = successfulProcessCount;

                LoadUids();

                Progress.TotalFilesToProcess = WorkQueueUidList.Count;
                Proxy.UpdateProgress(true);

                int maxBatch = WorkItemServiceSettings.Default.StudyProcessBatchSize;
                var fileList = new List<WorkItemUid>(maxBatch);
                
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

                    if (sop.FailureCount > 0)
                    {
                        // Failed SOPs we process individually
                        // All others we batch
                        if (fileList.Count > 0)
                        {
                            if (ProcessWorkQueueUids(fileList, studyXml))
                                successfulProcessCount++;
                            fileList = new List<WorkItemUid>();
                        }

                        fileList.Add(sop);

                        if (ProcessWorkQueueUids(fileList, studyXml))
                            successfulProcessCount++;

                        fileList = new List<WorkItemUid>();
                    }
                    else
                    {
                        fileList.Add(sop);

                        if (fileList.Count >= maxBatch)
                        {
                            // TODO (CR Jun 2012 - Med): This method indicates there is a relation between "process count" and the number
                            // of SOPs processed, but successfulProcessCount is only incremented by 1 for all the SOPs processed here.
                            // Will this unnecessarily slow processing down?
                            // Maybe ProcessWorkQueueUids should return the number processed successfully?

                            if (ProcessWorkQueueUids(fileList, studyXml))
                                successfulProcessCount++;

                            fileList = new List<WorkItemUid>();
                        }
                    }
                }

                if (fileList.Count > 0)
                {
                    if (ProcessWorkQueueUids(fileList, studyXml))
                        successfulProcessCount++;                 
                }
            }

            return successfulProcessCount;
        }

        /// <summary>
        /// Process a specified <see cref="WorkItemUid"/>
        /// </summary>
        /// <param name="sops">The <see cref="WorkItemUid"/> being processed</param>
        /// <param name="studyXml">The <see cref="StudyXml"/> object for the study being processed</param>
        /// <returns>true if the <see cref="WorkItemUid"/> is successfully processed. false otherwise</returns>
        protected virtual bool ProcessWorkQueueUids(List<WorkItemUid> sops, StudyXml studyXml)
        {
            Platform.CheckForNullReference(sops, "sops");
            Platform.CheckForNullReference(studyXml, "studyXml");

            string path = null;

            try
            {
                var fileList = new List<ProcessStudyUtility.ProcessorFile>();

                foreach (var uid in sops)
                {
                    path = !string.IsNullOrEmpty(uid.File) 
                        ? Path.Combine(Location.StudyFolder, uid.File) 
                        : Location.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);

                    fileList.Add(new ProcessStudyUtility.ProcessorFile(path,uid));
                }

                var processor = new ProcessStudyUtility(Location);

                processor.ProcessBatch(fileList, studyXml);

                Progress.NumberOfFilesProcessed += fileList.Count;
                Progress.StatusDetails = string.Empty;
                Proxy.UpdateProgress(true);
                Study = processor.StudyLocation.Study;
                return true;
            }
            catch (Exception e)
            {                
                foreach (var sop in sops)
                {
                    try
                    {
                        var updatedSop = FailWorkItemUid(sop, true);
                        sop.Failed = updatedSop.Failed;
                        sop.FailureCount = updatedSop.FailureCount;

                        Platform.Log(LogLevel.Error, e,
                                     "Unexpected exception when processing file: {0} SOP Instance: {1}",
                                     path ?? string.Empty,
                                     sop.SopInstanceUid);
                        Progress.StatusDetails = e.InnerException != null
                                                                ? String.Format("{0}:{1}", e.GetType().Name,
                                                                                e.InnerException.Message)
                                                                : String.Format("{0}:{1}", e.GetType().Name, e.Message);
                        if (sop.Failed)
                            Progress.NumberOfProcessingFailures++;
                    }
                    catch (Exception x)
                    {
                        Platform.Log(LogLevel.Error, "Unable to fail WorkItemUid {0}: {1}", sop.Oid, x.Message);
                    }
                }
                
                Proxy.UpdateProgress(true);  

                return false;
            }
        }
    }
}
