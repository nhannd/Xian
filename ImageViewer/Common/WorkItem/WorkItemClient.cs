using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.Auditing;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public abstract class WorkItemClient
    {
        public WorkItemData Request { get; set; }

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
            catch (Exception)
            {
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

    internal class ReindexApplication : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (new ReindexClient().Reindex())
                return;

            Console.WriteLine("Failed to start Re-index.");
            Environment.ExitCode = 1;
        }

        #endregion
    }

    public class ReindexClient : WorkItemClient
    {
        public bool Reindex()
        {
            var request = new ReindexRequest();

            try
            {
                InsertRequest(request);
                return true;
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, Common.SR.MessageFailedToStartReindex);
                return false;
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
