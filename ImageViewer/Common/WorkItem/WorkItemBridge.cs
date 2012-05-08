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
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public class WorkItemBridge
    {
        public WorkItemData WorkItem { get; set; }
        public WorkItemRequest Request { get; set; }
        public Exception Exception { get; set; }

        public void Cancel()
        {
            if (WorkItem == null)
                return;

            if (WorkItem.Progress != null && !WorkItem.Progress.IsCancelable) 
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
                                                                               {
                                                                                   Cancel = true, 
                                                                                   Identifier = WorkItem.Identifier
                                                                               }));
            WorkItem = response.Item;
        }

        public void Reset()
        {
            if (WorkItem == null)
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
                                                                               {                                                                                   
                                                                                   Status = WorkItemStatusEnum.Pending, 
                                                                                   ScheduledTime = Platform.Time, 
                                                                                   Identifier = WorkItem.Identifier
                                                                               }));
            WorkItem = response.Item;
        }

        public void Reprioritize(WorkItemPriorityEnum priority)
        {
            if (WorkItem == null)
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
            {
                Priority = priority,
                Identifier = WorkItem.Identifier,
                ScheduledTime = priority == WorkItemPriorityEnum.Stat ? Platform.Time : default(DateTime?)
            }));
            WorkItem = response.Item;
        }


        public void Delete()
        {
            if (WorkItem == null)
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
                                                                               {
                                                                                   Delete = true, // TODO (Marmot) - This delete flag could be removed, and we could just use the status
                                                                                   Identifier = WorkItem.Identifier
                                                                               }));        
            WorkItem = response.Item;
        }

        protected void InsertRequest(WorkItemRequest request)
        {
            WorkItemInsertResponse response = null;
            
            if(string.IsNullOrEmpty(request.UserName))
                request.UserName = GetUserName();
            
            Request = request;

            Platform.GetService<IWorkItemService>(s => response = s.Insert(new WorkItemInsertRequest { Request = request }));

            if (response == null) return;

            WorkItem = response.Item;
        }

        private static string GetUserName()
        {
            IPrincipal p = Thread.CurrentPrincipal;
            if (p == null || string.IsNullOrEmpty(p.Identity.Name))
                return string.Format("{0}@{1}", Environment.UserName, Environment.UserDomainName);
            return p.Identity.Name;
        }
    }

    public class DicomFileImportBridge : WorkItemBridge
    {
        public void ImportFileList(List<string> fileList,BadFileBehaviourEnum badFileBehaviour, FileImportBehaviourEnum fileImportBehavior )
        {

            var request = new ImportFilesRequest
            {
                FilePaths = fileList,
                Recursive = true,
                BadFileBehaviour = badFileBehaviour,
                FileImportBehaviour = fileImportBehavior
            };

            var result = EventResult.Success;

            try
            {
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = EventResult.MajorFailure;
                throw;
            }
            finally
            {
                AuditHelper.LogImportStudies(new AuditedInstances(), EventSource.CurrentUser, result);
            }
        }
    }

    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class ReindexApplication : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            try
            {
                var client = new ReindexFilestoreBridge();
                client.Reindex();
                Console.WriteLine("The re-index has been scheduled.");
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to start re-index.");
            }

            //TODO (Marmot): 
            Environment.ExitCode = 1;
        }

        #endregion
    }

    public class ReindexFilestoreBridge : WorkItemBridge
    {
        public void Reindex()
        {
            var request = new ReindexRequest();

            try
            {
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToStartReindex);
                throw;
            }
        }
    }

    public class ReapplyRulesBridge : WorkItemBridge
    {
        public void ReapplyRules(string ruleId, string ruleName, RulesEngineContext context)
        {
            var request = new ReapplyRulesRequest
                              {
                                  RuleId = ruleId,
                                  RuleName = ruleName,
                                  RulesEngineContext = context
                              };

            try
            {
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToStartReapplyRules);
                throw;
            }
        }
    }

    public class DeleteBridge : WorkItemBridge
    {
        public void DeleteStudy(IStudyRootData study)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DeleteStudyRequest
                                  {
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study)
                                  };

                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogDeleteStudies(AuditHelper.LocalAETitle, instances, EventSource.CurrentUser, result);
            }
        }

        public void DeleteSeries(IStudyRootData study, List<string> seriesInstanceUids)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DeleteSeriesRequest
                {
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study),
                    SeriesInstanceUids = seriesInstanceUids
                };

                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogUpdateInstances(new List<string> {AuditHelper.LocalAETitle}, instances, EventSource.CurrentUser, result);
            }
        }
    }

    public class DicomSendBridge : WorkItemBridge
    {
        public void MoveStudy(IDicomServiceNode remoteAEInfo, IStudyRootData study, WorkItemPriorityEnum priority)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomSendStudyRequest
                {
                    Destination = remoteAEInfo.Name,
                    Priority = priority,
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study)

                };

                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToSendStudy);
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogBeginSendInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName,
                                                  instances,
                                                  string.IsNullOrEmpty(Request.UserName)
                                                      ? EventSource.CurrentProcess
                                                      : EventSource.CurrentUser, result);
            }
        }

        public void MoveSeries(IDicomServiceNode remoteAEInfo, IStudyRootData study, string[] seriesInstanceUids, WorkItemPriorityEnum priority)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomSendSeriesRequest
                                  {
                                      Destination = remoteAEInfo.Name,
                                      SeriesInstanceUids = new List<string>(),
                                      Priority = priority,
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study)
                                  };

                request.SeriesInstanceUids.AddRange(seriesInstanceUids);
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogBeginSendInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName,
                                                  instances,
                                                  string.IsNullOrEmpty(Request.UserName)
                                                      ? EventSource.CurrentProcess
                                                      : EventSource.CurrentUser, result);
            }
        }

        public void MoveSops(IDicomServiceNode remoteAEInfo, IStudyRootData study, string seriesInstanceUid, string[] sopInstanceUids, WorkItemPriorityEnum priority)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomSendSopRequest
                                  {
                                      Destination = remoteAEInfo.Name,
                                      SeriesInstanceUid = seriesInstanceUid,
                                      SopInstanceUids = new List<string>(),
                                      Priority = priority,
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study)
                                  };
                request.SopInstanceUids.AddRange(sopInstanceUids);
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToSendSops);
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogBeginSendInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName,
                                                  instances,
                                                  string.IsNullOrEmpty(Request.UserName)
                                                      ? EventSource.CurrentProcess
                                                      : EventSource.CurrentUser, result);
            }
        }

        public void PublishFiles(IDicomServiceNode remoteAEInfo, IStudyRootData study, DeletionBehaviour behaviour, List<string> files)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new PublishFilesRequest
                                  {
                                      Destination = remoteAEInfo.Name,
                                      Priority = WorkItemPriorityEnum.High,
                                      DeletionBehaviour = behaviour,
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study),
                                      FilePaths = files
                                  };
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToSendSops);
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogBeginSendInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName,
                                                  instances,
                                                  string.IsNullOrEmpty(Request.UserName)
                                                      ? EventSource.CurrentProcess
                                                      : EventSource.CurrentUser, result);
            }
        }
    }

    public class DicomRetrieveBridge : WorkItemBridge
    {
        public void RetrieveStudy(IDicomServiceNode remoteAEInfo, IStudyRootData study)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomRetrieveStudyRequest
                {
                    Source = remoteAEInfo.Name,
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study)

                };

                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToSendStudy);
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogBeginReceiveInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName,
                                                     instances, string.IsNullOrEmpty(Request.UserName)
                                                                    ? EventSource.CurrentProcess
                                                                    : EventSource.CurrentUser, result);
            }
        }

        public void RetrieveSeries(IDicomServiceNode remoteAEInfo, IStudyRootData study, string[] seriesInstanceUids)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomRetrieveSeriesRequest
                {
                    Source = remoteAEInfo.Name,
                    SeriesInstanceUids = new List<string>(),
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study)
                };

                request.SeriesInstanceUids.AddRange(seriesInstanceUids);
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                result = EventResult.MajorFailure;
                Exception = ex;
                throw;
            }
            finally
            {
                var instances = new AuditedInstances();
                instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                AuditHelper.LogBeginReceiveInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName,
                                       instances, string.IsNullOrEmpty(Request.UserName)
                                                      ? EventSource.CurrentProcess
                                                      : EventSource.CurrentUser, result);
            }
        }
    }
}
