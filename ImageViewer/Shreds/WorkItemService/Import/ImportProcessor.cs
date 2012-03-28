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
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Import
{
    public class ImportProcessor : BaseItemProcessor<ImportFilesRequest,ImportFilesProgress>
    {
        public List<string> FilesToImport { get; set; }


        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            bool initResult = base.Initialize(proxy);
            
            FilesToImport = new List<string>();
            
            return initResult;
        }

        public override void Process()
        {
            ValidateRequest(Request);

            var filePaths = new List<string>(Request.FilePaths);

            Progress.StatusDetails = filePaths.Count > 1
                                       ? String.Format(SR.FormatMultipleFilesDescription, filePaths[0])
                                       : filePaths[0];
            Proxy.UpdateProgress();

            // Get a list of files to import
            LoadFileList(Request.FilePaths, Request.FileExtensions, Request.Recursive);

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
            if (FilesToImport.Count == 0)
            {
                Progress.StatusDetails = SR.MessageNoFilesToImport;
                Progress.IsCancelable = false;
            }
            else
            {
                ImportFiles();
            }

            if (CancelPending)
                Proxy.Cancel();
            else if (StopPending)
                Proxy.Postpone();
            else
                Proxy.Complete();
        }

        private void ValidateRequest(ImportFilesRequest filesRequest)
        {
            if (filesRequest == null)
                throw new ArgumentNullException(SR.ExceptionNoFilesHaveBeenSpecifiedToImport);

            if (filesRequest.FilePaths == null)
                throw new ArgumentNullException(SR.ExceptionNoFilesHaveBeenSpecifiedToImport);

            int paths = 0;

            foreach (string path in filesRequest.FilePaths)
            {
                if (Directory.Exists(path) || File.Exists(path))
                    ++paths;
            }

            if (paths == 0)
                throw new ArgumentNullException(SR.ExceptionNoValidFilesHaveBeenSpecifiedToImport);
        }


        private void LoadFileList(IEnumerable<string> filePaths,
            IEnumerable<string> fileExtensions,
            bool recursive)
        {
            // no impersonation of the job's user identity context is necessary inside the task delegate since
            // the .NET thread pool automatically captures the current execution context for you, which includes
            // the impersonated client credentials if you're calling this from the import service implementation

            var extensions = new List<string>();

            if (fileExtensions != null)
                foreach (string extension in fileExtensions)
                {
                    if (String.IsNullOrEmpty(extension))
                        continue;

                    extensions.Add(extension);
                }

            foreach (string path in filePaths)
            {
                FileProcessor.Process(path, "",
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
                                                  if (CancelPending)
                                                  {
                                                      return;
                                                  }

                                                  Progress.StatusDetails = String.Format(SR.FormatEnumeratingFile, file);

                                                  FilesToImport.Add(file);

                                                  ++Progress.TotalFilesToImport;
                                                  
                                                  Proxy.UpdateProgress();
                                              }

                                          }, recursive);

                if (CancelPending || StopPending)
                    break;
            }
        }

        private void ImportFiles()
        {
            var configuration = GetServerConfiguration();

            var context = new ImportStudyContext(configuration.AETitle);
            context.StudyWorkItems.ItemAdded += delegate(object sender, DictionaryEventArgs<string, WorkItem> e)
                                                    {
                                                        try
                                                        {
                                                            PublishManager<IWorkItemActivityCallback>.Publish(
                                                                "WorkItemChanged", WorkItemHelper.FromWorkItem(e.Item));
                                                        }
                                                        catch (Exception x)
                                                        {
                                                            Platform.Log(LogLevel.Warn, x,
                                                                         "Unexpected error attempting to publish WorkItem status");
                                                        }
                                                    };
            foreach (string file in FilesToImport)
            {
                try
                {
                    var dicomFile = new DicomFile(file);

                    DicomReadOptions readOptions = Request.FileImportBehaviour == FileImportBehaviourEnum.Save 
                                                       ? DicomReadOptions.Default
                                                       : DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences;
                    
                    dicomFile.Load(readOptions);

                    var importer = new SopInstanceImporter(context);

                    DicomProcessingResult result = importer.Import(dicomFile,Request.BadFileBehaviour,Request.FileImportBehaviour);

                    if (result.DicomStatus == DicomStatuses.Success)
                    {
                        Progress.NumberOfFilesImported++;
                    }
                    if (CancelPending || StopPending)
                        return;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Unable to import DICOM File: {0}", file);
                    Progress.NumberOfImportFailures++;
                }

                Proxy.UpdateProgress();
            }
        }

        public override bool CanStart(out string reason)
        {
            reason = string.Empty;
            // TODO: Check for ReIndex pending job?
            return true;
        }
    }
}
