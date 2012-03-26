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
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Import
{
    public class ImportProcessor : BaseItemProcessor
    {
        public List<string> FilesToImport { get; set; }

        public ImportWorkItemProgress Progress
        {
            get { return Proxy.Item.Progress as ImportWorkItemProgress; }
        }

        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            if (proxy.Item.Progress == null)
                proxy.Item.Progress = new ImportWorkItemProgress();
            else if (!(proxy.Item.Progress is ImportWorkItemProgress))
                proxy.Item.Progress = new ImportWorkItemProgress();
            
            FilesToImport = new List<string>();

            return base.Initialize(proxy);
        }

        public override void Process(WorkItemStatusProxy proxy)
        {
            var request = proxy.Item.Request as DicomImportRequest;
            if (request == null)
                throw new Exception("Internal error, ImportRequest in invalid format");

            ValidateRequest(request);

            var filePaths = new List<string>(request.FilePaths);

            Progress.Description = filePaths.Count > 1
                                       ? String.Format(SR.FormatMultipleFilesDescription, filePaths[0])
                                       : filePaths[0];

            // Get a list of files to import
            LoadFileList(request.FilePaths, request.FileExtensions, request.Recursive);

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
                Progress.StatusDescription = SR.MessageNoFilesToImport;
                Progress.IsCancelable = false;
            }
            else
            {
                ImportFiles();
            }


            if (CancelPending)
                Proxy.Cancel();
            else if (StopPending)
                proxy.Postpone();
            else
                Proxy.Complete();
        }

        private void ValidateRequest(DicomImportRequest request)
        {
            if (request.FilePaths == null)
                throw new ArgumentNullException(SR.ExceptionNoFilesHaveBeenSpecifiedToImport);

            int paths = 0;
            int badPaths = 0;

            foreach (string path in request.FilePaths)
            {
                if (Directory.Exists(path) || File.Exists(path))
                    ++paths;
                else
                    ++badPaths;
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

            foreach (string extension in fileExtensions)
            {
                if (String.IsNullOrEmpty(extension))
                    continue;

                extensions.Add(extension);
            }


            bool cancelled = false;

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

                                                  Progress.StatusDescription = String.Format(SR.FormatEnumeratingFile, file);

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
            // TODO: Get real AE Title
            var context = new ImportStudyContext("AE");

            foreach (string file in FilesToImport)
            {
                try
                {
                    var dicomFile = new DicomFile(file);

                    dicomFile.Load();

                    var importer = new SopInstanceImporter(context);

                    importer.Import(dicomFile);

                    Progress.NumberOfFilesImported++;

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

        public override void Delete(WorkItemStatusProxy proxy)
        {
            // Just delete, nothing else to do.
            Proxy.Delete();
        }

        protected override bool CanStart()
        {
            // TODO: Check for ReIndex pending job?
            return true;
        }
    }
}
