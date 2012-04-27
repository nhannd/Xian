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
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.ProcessStudy
{
    /// <summary>
    /// Processor for <see cref="WorkItemTypeEnum.ProcessStudy"/> entries.
    /// </summary>
    public class StudyProcessProcessor : BaseItemProcessor<ProcessStudyRequest,ProcessStudyProgress>
    {
        public Study Study { get; set; }

        /// <summary>
        /// Cleanup any failed items in the queue and delete the queue entry.
        /// </summary>
        public override void Delete()
        {
            LoadUids();
            foreach (WorkItemUid sop in WorkQueueUidList)
            {
                if (sop.Failed)
                {
                    try
                    {
                        string file = string.IsNullOrEmpty(sop.File)
                                          ? Location.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid)
                                          : Path.Combine(Location.StudyFolder, sop.File);
                        FileUtils.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception attempting to cleanup file for Work Item {0}",
                                     Proxy.Item.Oid);
                    }
                }
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
                    Proxy.Fail(WorkItemFailureType.NonFatal);
                else if (!complete)
                    Proxy.Postpone();
                else if (now > Proxy.Item.ExpirationTime)
                {
                    var ep = new RulesEngineExtensionPoint();
                    if (Study == null)
                        Study = LoadRelatedStudy();

                    var ruleContext = new RulesEngineContext
                                          {
                                              ApplyDeleteActions = true,
                                              ApplySendStudyActions = true
                                          };
                    StudyEntry studyEntry = Study.ToStoreEntry();
                    foreach (IRulesEngine engine in ep.CreateExtensions())
                    {
                        engine.ApplyStudyRules(ruleContext, studyEntry, WorkItemHelper.FromWorkItem( Proxy.Item));
                    }

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

        /// <summary>
        /// Can the <see cref="WorkItem"/> start processing?
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public override bool CanStart(out string reason)
        {
            var relatedList = FindRelatedWorkItems(null, new List<WorkItemStatusEnum> {WorkItemStatusEnum.InProgress});
            
            reason = string.Empty;

            if (relatedList.Count > 0)
            {
                reason = "There are related WorkItems for the study being processed.";
                return false;
            }
            return true;
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
                if (lastSuccessProcessCount != -1 && (successfulProcessCount - lastSuccessProcessCount) < WorkItemServiceSettings.Instance.StudyProcessBatchSize)
                    break;

                lastSuccessProcessCount = successfulProcessCount;

                LoadUids();

                Progress.TotalFilesToProcess = WorkQueueUidList.Count;
                Proxy.UpdateProgress();

                int maxBatch = WorkItemServiceSettings.Instance.StudyProcessBatchSize;
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
                Proxy.UpdateProgress();
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
                
                Proxy.UpdateProgress();  

                return false;
            }
        }
    }
}
