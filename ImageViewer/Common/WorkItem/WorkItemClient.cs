using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Common.Auditing;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public abstract class WorkItemClient
    {
        protected WorkItemData InsertRequest(WorkItemRequest request)
        {
            //TODO Marmot, use a local service provider
            var result = WorkItemMonitor.Insert(new WorkItemInsertRequest {Request = request});

            return result.Item;
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

                var message = string.Format("Failed to connect to the local data store service to import files.  The files must be imported manually (location: {0})", importFolder);
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

                var message = string.Format("Failed to connect to the local data store service to import files.  The files must be imported manually (location: {0})", importFolder);
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
}
