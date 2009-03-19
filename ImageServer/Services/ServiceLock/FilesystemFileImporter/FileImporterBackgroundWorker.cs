using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    internal class DirectoryImporterParameters
    {
        public String PartitionAE;
        public DirectoryInfo Directory;
        public int MaxImages;
        public int Delay;
        public string Filter;
    }

    /// <summary>
    /// Background worker thread to import dicom files from a directory.
    /// </summary>
    internal class DirectoryImporterBackgroundProcess : BackgroundWorker
    {
        #region Private Fields
        private DirectoryImporterParameters _parms;
        SopInstanceImporter _importer;
        #endregion

        #region Constructors
        public DirectoryImporterBackgroundProcess(DirectoryImporterParameters parms)
        {
            _parms = parms;
            WorkerSupportsCancellation = true;
        }
        #endregion

        #region Protected Methods

        
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = String.Format("Import File to {0}", _parms.PartitionAE);
            base.OnDoWork(e);

            if (_parms.Directory.Exists)
            {
                _importer = new SopInstanceImporter(_parms.PartitionAE);
                
                int counter = 0;
                Platform.Log(LogLevel.Info, "Importing dicom files from {0}", _parms.Directory.FullName);
                FileProcessor.Process(_parms.Directory.FullName, _parms.Filter,
                                      delegate(string filePath, out bool cancel)
                                      {
                                          if (CancellationPending || counter >= _parms.MaxImages)
                                          {
                                              cancel = true;
                                              return;
                                          }

                                          ProcessFile(filePath);

                                          cancel = false;
                                          counter++;

                                          if (_parms.Delay > 0)
                                            Thread.Sleep(TimeSpan.FromSeconds(_parms.Delay));
                                          
                                      }, true);


                DirectoryInfo[] subDirs = _parms.Directory.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    DirectoryUtility.DeleteEmptySubDirectories(subDir.FullName, true);
                    DirectoryUtility.DeleteIfEmpty(subDir.FullName);
                } 
            }
            
        }

        private void ProcessFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                try
                {
                    DicomFile file = new DicomFile(filePath);
                    file.Load();
                    DicomSopProcessingResult result = _importer.Import(file, "Importer");
                    if (result.Sussessful)
                    {
                        Platform.Log(LogLevel.Info, "Imported SOP {0} to {1}", result.SopInstanceUid, _parms.PartitionAE);
                        ProgressChangedEventArgs progress = new ProgressChangedEventArgs(100, result.SopInstanceUid);
                        OnProgressChanged(progress);
                    }
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                }

                fileInfo.Delete();
            }
        }
    

        #endregion
    }
}