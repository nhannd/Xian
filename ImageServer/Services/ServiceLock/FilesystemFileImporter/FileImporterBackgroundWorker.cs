using System;
using System.ComponentModel;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    internal class DirectoryImporterParameters
    {
        public String PartitionAE;
        public DirectoryInfo Directory;
        public int MaxImages;
        public string Filter;
    }

    /// <summary>
    /// Background worker thread to import dicom files from a directory.
    /// </summary>
    internal class DirectoryImporterBackgroundProcess : BackgroundWorker
    {
        #region Private Fields
        private DirectoryImporterParameters _parms;
        #endregion

        #region Constructors
        public DirectoryImporterBackgroundProcess(DirectoryImporterParameters parms)
        {
            _parms = parms;
            WorkerSupportsCancellation = true;
            ProgressChanged += new ProgressChangedEventHandler(FileImporterBackgroundProcess_ProgressChanged);
        }
        #endregion

        #region Protected Methods

        protected void FileImporterBackgroundProcess_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Platform.Log(LogLevel.Info, "Imported SOP {0} to {1}", e.UserState as String, _parms.PartitionAE);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            base.OnDoWork(e);
            Platform.Log(LogLevel.Info, "Importing dicom files from {0}", _parms.Directory.FullName);
            int counter = 0;
            FileProcessor.Process(_parms.Directory.FullName,  _parms.Filter,
                                  delegate(string filePath, out bool cancel)
                                      {
                                          if (CancellationPending || counter >= _parms.MaxImages)
                                          {
                                              cancel = true;
                                              return;
                                          }
                                          cancel = false;
                                          
                                          SopInstanceImporter importer = new SopInstanceImporter(_parms.PartitionAE);
                                          DicomFile file = new DicomFile(filePath);
                                          file.Load();
                                          DicomSopProcessingResult result = importer.Import(file, "Importer");
                                          if (result.Sussessful)
                                          {
                                              ProgressChangedEventArgs progress = new ProgressChangedEventArgs(100, result.SopInstanceUid);
                                              OnProgressChanged(progress);
                                          }
                                          File.Delete(filePath);
                                          counter++;
                                      }, true);
        }

        #endregion
    }
}