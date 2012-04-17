using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.Auditing;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public class WorkItemClient
    {
        public WorkItemData Request { get; set; }
        public Exception Exception { get; set; }

        public void Cancel()
        {
            if (Request == null)
                return;

            if (Request.Status == WorkItemStatusEnum.Canceled) 
                return;

            if (Request.Progress == null || !Request.Progress.IsCancelable) 
                return;

            WorkItemUpdateResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Update(new WorkItemUpdateRequest {Cancel = true, Identifier = Request.Identifier}));

            // Cancel the request
            Request = response.Item;
        }

        protected void InsertRequest(WorkItemRequest request)
        {
            WorkItemInsertResponse response = null;

            Platform.GetService<IWorkItemService>(s => response = s.Insert(new WorkItemInsertRequest { Request = request }));

            if (response == null) return;

            Request = response.Item;
        }
    }

    public class DicomFileImportClient : WorkItemClient
    {
        public void PublishLocal(string importFolder, AuditedInstances auditedInstances, BadFileBehaviourEnum badFileBehavior, FileImportBehaviourEnum fileImportBehavior)
        {          
            // setup auditing information
            var result = EventResult.Success;
     
            var request = new ImportFilesRequest
            {
                FilePaths = new List<string> { importFolder },
                Recursive = true,
                BadFileBehaviour = badFileBehavior,
                FileImportBehaviour = fileImportBehavior
            };

            try
            {
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = EventResult.MajorFailure;

                var message = string.Format("Failed to connect to the work item service to import files.  The files must be imported manually (location: {0})", importFolder);
                throw new DicomFilePublishingException(message, ex);
            }
            finally
            {
                //TODO (Marmot) Move this to the SopInstanceImporter & pass the current user through the Request?
                // audit attempt to import these instances to local store
                AuditHelper.LogImportStudies(auditedInstances, EventSource.CurrentUser, result);
            }
        }

        public void CreateInstances(string importFolder, AuditedInstances auditedInstances, BadFileBehaviourEnum badFileBehavior, FileImportBehaviourEnum fileImportBehavior)
        {
            // setup auditing information
            var result = EventResult.Success;

            var request = new ImportFilesRequest
            {
                FilePaths = new List<string> { importFolder },
                Recursive = true,
                BadFileBehaviour = badFileBehavior,
                FileImportBehaviour = fileImportBehavior
            };

            try
            {
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = EventResult.MajorFailure;

                var message = string.Format("Failed to connect to the work item service to import files.  The files must be imported manually (location: {0})", importFolder);
                throw new DicomFilePublishingException(message, ex);
            }
            finally
            {
                //TODO (Marmot) Move this to the SopInstanceImporter & pass the current user through the Request?
                AuditHelper.LogCreateInstances(new string[0], auditedInstances, EventSource.CurrentUser, result);
            }
        }

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
                //TODO (Marmot) Move this to the SopInstanceImporter & pass the current user through the Request?
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
                var client = new ReindexClient();
                client.Reindex();
                Console.WriteLine("The re-index has been scheduled.");
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to start re-index.");
            }

            Environment.ExitCode = 1;
        }

        #endregion
    }

    public class ReindexClient : WorkItemClient
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

    public class DicomSendClient : WorkItemClient
    {
        public void MoveStudy(ApplicationEntity remoteAEInfo, IStudyRootStudyIdentifier study, WorkItemPriorityEnum priority)
        {
            try
            {
                var request = new DicomSendStudyRequest
                {
                    AeTitle = remoteAEInfo.AETitle,
                    Host = remoteAEInfo.ScpParameters.HostName,
                    Port = remoteAEInfo.ScpParameters.Port,
                    Priority = priority,
                    Study = new WorkItemStudy(study),
                    Patient = new WorkItemPatient(study)

                };
                
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToSendStudy);
                throw;
            }
        }

        public void MoveSeries(ApplicationEntity remoteAEInfo, IStudyRootStudyIdentifier study, string[] seriesInstanceUids, WorkItemPriorityEnum priority)
        {
            try
            {
                var request = new DicomSendSeriesRequest
                                  {
                                      AeTitle = remoteAEInfo.AETitle,
                                      Host = remoteAEInfo.ScpParameters.HostName,
                                      Port = remoteAEInfo.ScpParameters.Port,
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
                Exception = ex;
                throw;
            }
        }

        public void MoveSops(ApplicationEntity remoteAEInfo, IStudyRootStudyIdentifier study, string seriesInstanceUid, string[] sopInstanceUids, WorkItemPriorityEnum priority)
        {
            try
            {
                var request = new DicomSendSopRequest
                                  {
                                      AeTitle = remoteAEInfo.AETitle,
                                      Host = remoteAEInfo.ScpParameters.HostName,
                                      Port = remoteAEInfo.ScpParameters.Port,
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
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToSendSops);
                throw;
            }
        }

        public void PublishFiles(ApplicationEntity remoteAEInfo, IStudyRootStudyIdentifier study, DeletionBehaviour behaviour, List<string> files  )
        {
            try
            {
                var request = new PublishFilesRequest
                                  {
                                      AeTitle = remoteAEInfo.AETitle,
                                      Host = remoteAEInfo.ScpParameters.HostName,
                                      Port = remoteAEInfo.ScpParameters.Port,
                                      Priority = WorkItemPriorityEnum.Stat,
                                      DeletionBehaviour = behaviour,
                                      Study = new WorkItemStudy(study),
                                      Patient = new WorkItemPatient(study),
                                      FilePaths = files
                                  };
                InsertRequest(request);
            }
            catch (Exception ex)
            {
                Exception = ex;
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToSendSops);
                throw;
            }
        }
    }

    public class WorkItemPublisher
    {
        public static void Publish(WorkItemData workItem)
        {
            try
            {            
                Platform.GetService<IWorkItemService>(s => s.Publish(new WorkItemPublishRequest { Item = workItem }));
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Failed to Publish WorkItem status");
            }     
        }
    }
}
