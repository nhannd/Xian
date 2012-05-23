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
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Import
{
    /// <summary>
    /// Processor for import of files.
    /// </summary>
    internal class ImportItemProcessor : BaseItemProcessor<ImportFilesRequest,ImportFilesProgress>
    {
        #region Public Methods
        
        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            bool initResult = base.Initialize(proxy);

            return initResult;
        }

        public override void Process()
        {
            if (!ValidateRequest(Request))
            {
                Proxy.Fail(SR.ExceptionNoValidFilesHaveBeenSpecifiedToImport, WorkItemFailureType.Fatal);
                return;
            }

            Progress.NumberOfFilesImported = 0;
            Progress.NumberOfImportFailures = 0;
            Progress.PathsImported = 0;
            Progress.PathsToImport = 0;

            Progress.StatusDetails = Request.FilePaths.Count > 1
                                       ? String.Format(SR.FormatMultipleFilesDescription, Request.FilePaths[0])
                                       : Request.FilePaths[0];
            Proxy.UpdateProgress();

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

            //it's ok to read this property unsynchronized because this is the only thread that is adding to the queue for the particular job.
            if (Request.FilePaths.Count == 0)
            {
                Progress.StatusDetails = SR.MessageNoFilesToImport;
                Progress.IsCancelable = false;
            }
            else
            {
                ImportFiles(Request.FilePaths, Request.FileExtensions, Request.Recursive);
                GC.Collect();
            }

            if (CancelPending)
                Proxy.Cancel();
            else if (StopPending)
                Proxy.Postpone();
            else if (Progress.NumberOfImportFailures > 0)
                Proxy.Fail(WorkItemFailureType.Fatal);
            else
                Proxy.Complete();
        }

        public override bool CanStart(out string reason)
        {
            reason = string.Empty;

            return !ReindexScheduled();
        }

        #endregion

        #region Private Methods

        private bool ValidateRequest(ImportFilesRequest filesRequest)
        {
            if (filesRequest == null)
                return false;

            if (filesRequest.FilePaths == null)
                return false;

            int paths = 0;

            foreach (string path in filesRequest.FilePaths)
            {
                if (Directory.Exists(path) || File.Exists(path))
                    ++paths;
            }

            if (paths == 0)
                return false;

            return true;
        }

        private void ImportFile(string file, ImportStudyContext context)
        {
            // Note, we're not doing impersonation of the user's identity, so we may have failures here
            // which would be new in Marmot.
            try
            {
                var dicomFile = new DicomFile(file);

                DicomReadOptions readOptions = Request.FileImportBehaviour == FileImportBehaviourEnum.Save
                                                   ? DicomReadOptions.Default
                                                   : DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences;

                dicomFile.Load(readOptions);

                var importer = new ImportFilesUtility(context);

                DicomProcessingResult result = importer.Import(dicomFile, Request.BadFileBehaviour, Request.FileImportBehaviour);

                if (result.DicomStatus == DicomStatuses.Success)
                {
                    Progress.NumberOfFilesImported++;
                }
                else
                {
                    Progress.NumberOfImportFailures++;
                    Progress.StatusDetails = result.ErrorMessage;
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, "Unable to import DICOM File ({0}): {1}", file, e.Message);
                Progress.NumberOfImportFailures++;
            }
        }

        private void ImportFiles(IList<string> filePaths,
            IEnumerable<string> fileExtensions,
            bool recursive)
        {
            var configuration = GetServerConfiguration();

            var context = new ImportStudyContext(configuration.AETitle, StudyStore.GetConfiguration());

            // Publish the creation of the StudyImport WorkItems
            context.StudyWorkItems.ItemAdded += (sender, args) => Platform.GetService(
                (IWorkItemActivityMonitorService service) =>
                service.Publish(new WorkItemPublishRequest {Item = WorkItemDataHelper.FromWorkItem(args.Item)}));

            var extensions = new List<string>();

            if (fileExtensions != null)
                foreach (string extension in fileExtensions)
                {
                    if (String.IsNullOrEmpty(extension))
                        continue;

                    extensions.Add(extension);
                }

            Progress.PathsToImport = filePaths.Count;

            foreach (string path in filePaths)
            {
                FileProcessor.Process(path, string.Empty,
                                      delegate(string file, out bool cancel)
                                      {
                                          cancel = false;

                                          bool enqueue = false;
                                          foreach (string extension in extensions)
                                          {
                                              if (file.EndsWith(extension))
                                              {
                                                  enqueue = true;
                                                  break;
                                              }
                                          }

                                          enqueue = enqueue || extensions.Count == 0;

                                          if (enqueue)
                                          {
                                              if (CancelPending || StopPending)
                                              {
                                                  return;
                                              }

                                              Progress.StatusDetails = String.Format(SR.FormatEnumeratingFile, file);

                                              ++Progress.TotalFilesToImport;
                                              
                                              Proxy.UpdateProgress();

                                              ImportFile(file, context);                                                                                                                                                                                        
                                          }

                                      }, recursive);

                Progress.PathsImported++;
                Proxy.UpdateProgress();

                if (CancelPending || StopPending)
                    break;
            }
        }

        #endregion
    }
}
