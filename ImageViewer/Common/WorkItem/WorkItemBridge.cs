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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common.Auditing;

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

            if (WorkItem.Status == WorkItemStatusEnum.Deleted)
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
                                                                               {
                                                                                   Cancel = true, 
                                                                                   Identifier = WorkItem.Identifier
                                                                               }));
            
            // TODO (CR Jun 2012): The passed-in WorkItem contract should not be updated;
            // it should be done by the service and a new instance returned, or something should be returned by this
            // method to let the caller decide what to do.
            if (response.Item == null)
                WorkItem.Status = WorkItemStatusEnum.Deleted;
            else
                WorkItem = response.Item;
        }

        public void Reset()
        {
            if (WorkItem == null)
                return;

            if (WorkItem.Status == WorkItemStatusEnum.Deleted)
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
                                                                               {                                                                                   
                                                                                   Status = WorkItemStatusEnum.Pending, 
                                                                                   ProcessTime = Platform.Time, 
                                                                                   Identifier = WorkItem.Identifier
                                                                               }));
            // TODO (CR Jun 2012): The passed-in WorkItem contract should not be updated;
            // it should be done by the service and a new instance returned, or something should be returned by this
            // method to let the caller decide what to do.

            if (response.Item == null)
                WorkItem.Status = WorkItemStatusEnum.Deleted;
            else
                WorkItem = response.Item;
        }

        public void Reprioritize(WorkItemPriorityEnum priority)
        {
            if (WorkItem == null)
                return;

            if (WorkItem.Status == WorkItemStatusEnum.Deleted)
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
            {
                Priority = priority,
                Identifier = WorkItem.Identifier,
                ProcessTime = priority == WorkItemPriorityEnum.Stat ? Platform.Time : default(DateTime?)
            }));

            // TODO (CR Jun 2012): The passed-in WorkItem contract should not be updated;
            // it should be done by the service and a new instance returned, or something should be returned by this
            // method to let the caller decide what to do.

            if (response.Item == null)
                WorkItem.Status = WorkItemStatusEnum.Deleted;
            else
                WorkItem = response.Item;
        }


        public void Delete()
        {
            if (WorkItem == null)
                return;

            if (WorkItem.Status == WorkItemStatusEnum.Deleted)
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest
                                                                               {
                                                                                   Delete = true, // TODO (Marmot) - This delete flag could be removed, and we could just use the status
                                                                                   Identifier = WorkItem.Identifier
                                                                               }));

            // TODO (CR Jun 2012): The passed-in WorkItem contract should not be updated;
            // it should be done by the service and a new instance returned, or something should be returned by this
            // method to let the caller decide what to do.

            if (response.Item == null)
                WorkItem.Status = WorkItemStatusEnum.Deleted;
            else
                WorkItem = response.Item;
        }

        protected void InsertRequest(WorkItemRequest request, WorkItemProgress progress)
        {
            WorkItemInsertResponse response = null;

            // TODO (CR Jun 2012): This used?
            // (SW) This was intended to be used for auditing purposes on the server side, and may be needed to get auditing working properly
            if(string.IsNullOrEmpty(request.UserName))
                request.UserName = GetUserName();
            
            Request = request;

            Platform.GetService<IWorkItemService>(s => response = s.Insert(new WorkItemInsertRequest { Request = request, Progress = progress}));

            // TODO (CR Jun 2012): The passed-in WorkItem contract should not be updated;
            // it should be done by the service and a new instance returned, or something should be returned by this
            // method to let the caller decide what to do.

            if (response.Item == null)
                WorkItem.Status = WorkItemStatusEnum.Deleted;
            else
                WorkItem = response.Item;
        }

        // TODO (CR Jun 2012): Name? It's not returning a request, but a WorkItemData object.
        protected WorkItemData GetMatchingRequest(WorkItemRequest request)
        {
            WorkItemData returnedItem = null;

            Platform.GetService(delegate(IWorkItemService s)
                                    {
                                        var response = s.Query(new WorkItemQueryRequest
                                                                   {
                                                                       Type = request.WorkItemType,
                                                                       StudyInstanceUid =
                                                                           (request is WorkItemStudyRequest)
                                                                               ? (request as WorkItemStudyRequest).Study.StudyInstanceUid
                                                                               : null
                                                                   });

                                        // TODO (CR Jun 2012): The name of this method doesn't seem to describe what it returns.
                                        // Is it returning the first "active" work item for a study?
                                        foreach (var relatedItem in response.Items)
                                        {
                                            if (relatedItem.Status == WorkItemStatusEnum.Idle
                                                || relatedItem.Status == WorkItemStatusEnum.Pending
                                                || relatedItem.Status == WorkItemStatusEnum.InProgress)
                                            {
                                                returnedItem = relatedItem;
                                                break;
                                            }
                                        }
                                    });

            return returnedItem;
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
                InsertRequest(request, null);
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
        private class CmdLine : CommandLine
        {
            public CmdLine()
            {
                Timeout = 30;
            }

            [CommandLineParameter("Timeout", "t", "Specifies the amount of time, in seconds, to wait for the re-index to be started before quitting.")]
            public int Timeout { get; set; }
        }

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            var cmdLine = new CmdLine();
            try
            {
                cmdLine.Parse(args);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Info, e);
                Console.WriteLine(e.Message);
                cmdLine.PrintUsage(Console.Out);
                Environment.Exit(-1);
            }

            int timeoutMillisecondsRemaining = cmdLine.Timeout*1000;
            int tryCount = 0;
            while (timeoutMillisecondsRemaining > 0)
            {
                ++tryCount;
                if (tryCount > 1)
                {
                    Platform.Log(LogLevel.Info, "Previous attempt to start re-index failed - trying again (attempt #{0})", tryCount);
                    Console.WriteLine("Previous attempt to start re-index failed - trying again (attempt #{0})", tryCount);
                }

                int startTicks = Environment.TickCount;

                try
                {
                    var client = new ReindexFilestoreBridge();
                    client.Reindex();
                    Console.WriteLine("The re-index has been scheduled.");
                    Environment.ExitCode = 0;
                    return;
                }
                catch (EndpointNotFoundException)
                {
                    Platform.Log(LogLevel.Warn, "Failed to start re-index because the Work Item service isn't running.");
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Failed to start re-index.");
                    Console.WriteLine("Failed to start re-index.");
                }
                finally
                {
                    Thread.Sleep(2000);
                    var elapsedMilliseconds = Environment.TickCount - startTicks;
                    timeoutMillisecondsRemaining -= elapsedMilliseconds;
                }
            }

            Platform.Log(LogLevel.Warn, "Unable to start re-index after {0} attempts", tryCount);
            Console.WriteLine("Unable to start re-index after {0} attempts", tryCount);

            Environment.ExitCode = -1;
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
                InsertRequest(request, null);
            }
            catch (Exception ex)
            {
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToStartReindex);
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

                InsertRequest(request, new DeleteProgress());
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

                InsertRequest(request, new DeleteProgress());
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

                AuditHelper.LogDeleteSeries(new List<string> {AuditHelper.LocalAETitle}, instances, EventSource.CurrentUser, result);
            }
        }
    }

    public class DicomSendBridge : WorkItemBridge
    {
        // TODO (CR Jun 2012): Name?
        public void MoveStudy(IDicomServiceNode remoteAEInfo, IStudyRootData study, WorkItemPriorityEnum priority)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomSendStudyRequest
                {
                    DestinationServerName = remoteAEInfo.Name,
                    Priority = priority,
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study)

                };

                InsertRequest(request, null);
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

        // TODO (CR Jun 2012): Name?
        public void MoveSeries(IDicomServiceNode remoteAEInfo, IStudyRootData study, string[] seriesInstanceUids, WorkItemPriorityEnum priority)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomSendSeriesRequest
                                  {
                                      DestinationServerName = remoteAEInfo.Name,
                                      SeriesInstanceUids = new List<string>(),
                                      Priority = priority,
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study)
                                  };

                request.SeriesInstanceUids.AddRange(seriesInstanceUids);
                InsertRequest(request, null);
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

        // TODO (CR Jun 2012): Name
        public void MoveSops(IDicomServiceNode remoteAEInfo, IStudyRootData study, string seriesInstanceUid, string[] sopInstanceUids, WorkItemPriorityEnum priority)
        {
            EventResult result = EventResult.Success;
            try
            {
                var request = new DicomSendSopRequest
                                  {
                                      DestinationServerName = remoteAEInfo.Name,
                                      SeriesInstanceUid = seriesInstanceUid,
                                      SopInstanceUids = new List<string>(),
                                      Priority = priority,
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study)
                                  };
                request.SopInstanceUids.AddRange(sopInstanceUids);
                InsertRequest(request, null);
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
                                      DestinationServerName = remoteAEInfo.Name,
                                      Priority = WorkItemPriorityEnum.High,
                                      DeletionBehaviour = behaviour,
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study),
                                      FilePaths = files
                                  };
                InsertRequest(request, null);
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
                    ServerName = remoteAEInfo.Name,
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study)

                };

                var data = GetMatchingRequest(request);
                if (data != null)
                {
                    var existingRequest = data.Request as DicomRetrieveStudyRequest;
                    if (existingRequest != null && remoteAEInfo.Name == existingRequest.ServerName)
                    {
                        Request = data.Request;
                        return;
                    }
                }

                InsertRequest(request, new DicomRetrieveProgress());
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
                    ServerName = remoteAEInfo.Name,
                    SeriesInstanceUids = new List<string>(),
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study)
                };

                request.SeriesInstanceUids.AddRange(seriesInstanceUids);
                InsertRequest(request, new DicomRetrieveProgress());
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

	public class ReapplyRulesBridge : WorkItemBridge
	{
		/// <summary>
		/// Reapply specified rule.
		/// </summary>
		/// <param name="ruleId"></param>
		/// <param name="ruleName"></param>
		/// <param name="context"></param>
		public void Reapply(string ruleId, string ruleName, RulesEngineOptions context)
		{
			var request = new ReapplyRulesRequest
			{
				RuleId = ruleId,
				RuleName = ruleName,
				ApplyDeleteActions = context.ApplyDeleteActions,
				ApplyRouteActions = context.ApplyRouteActions
			};

			try
			{
				InsertRequest(request, new ReapplyRulesProgress());
			}
			catch (Exception ex)
			{
				Exception = ex;
				Platform.Log(LogLevel.Error, ex, SR.MessageFailedToStartReapplyRules);
				throw;
			}
		}

		/// <summary>
		/// Reapply all rules.
		/// </summary>
		/// <param name="context"></param>
		public void ReapplyAll(RulesEngineOptions context)
		{
			Reapply(null, null, context);
		}
	}
}
