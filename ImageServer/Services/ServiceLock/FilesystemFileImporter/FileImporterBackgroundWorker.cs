using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
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

    internal class SopImportedEventArgs : EventArgs
    {
        public string  StudyInstanceUid;
        public string  SeriesInstanceUid;
        public string  SopInstanceUid;
    }

    /// <summary>
    /// Background worker thread to import dicom files from a directory.
    /// </summary>
    internal class DirectoryImporterBackgroundProcess : BackgroundWorker
    {
        #region Private Fields
        private readonly DirectoryImporterParameters _parms;
        SopInstanceImporter _importer;
        private readonly List<string> _skippedStudies = new List<String>();
        private  EventHandler<SopImportedEventArgs> _sopImportedHandlers;
        #endregion

        #region Constructors
        public DirectoryImporterBackgroundProcess(DirectoryImporterParameters parms)
        {
            Platform.CheckForNullReference(parms, "parms");
            Platform.CheckMemberIsSet(parms.Directory, "parms.Directory");
            Platform.CheckMemberIsSet(parms.PartitionAE, "parms.PartitionAE");
            Platform.CheckMemberIsSet(parms.Filter, "parms.Filter");
            
            _parms = parms;
            WorkerSupportsCancellation = true;
        }
        #endregion

        #region Events
        public event EventHandler<SopImportedEventArgs> SopImported
        {
            add { _sopImportedHandlers += value; }
            remove { _sopImportedHandlers -= value; }
        }
        #endregion

        #region Protected Methods


        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = String.Format("Import Files to {0}", _parms.PartitionAE);
            
            if (_parms.Directory.Exists)
            {
                _importer = new SopInstanceImporter(_parms.PartitionAE);
                
                int counter = 0;
                Platform.Log(LogLevel.Debug, "Importing dicom files from {0}", _parms.Directory.FullName);
                FileProcessor.Process(_parms.Directory.FullName, _parms.Filter,
                                      delegate(string filePath, out bool cancel)
                                      {
                                          if (CancellationPending || counter >= _parms.MaxImages)
                                          {
                                              cancel = true;
                                              return;
                                          }

                                          if (ProcessFile(filePath)> 0)
                                          {

                                              counter++;
                                          }

                                          cancel = false;
                                          if (_parms.Delay > 0)
                                            Thread.Sleep(TimeSpan.FromSeconds(_parms.Delay));
                                          
                                      }, true);

                if (counter > 0)
                    Platform.Log(LogLevel.Info, "{0} files have been successfully imported from {1}.", counter, _parms.Directory.FullName);

                DirectoryInfo[] subDirs = _parms.Directory.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    DirectoryUtility.DeleteEmptySubDirectories(subDir.FullName, true);
                    DirectoryUtility.DeleteIfEmpty(subDir.FullName);
                }

                
            }

            base.OnDoWork(e);
        }

        private int ProcessFile(string filePath)
        {
            int importedSopCount = 0;
            bool isDicomFile = false;
            bool skipped = false;
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                DicomFile file;
                try
                {
                    file = new DicomFile(filePath);
                    file.Load();
                    isDicomFile = true;

                    string studyInstanceUid;
                    if (file.DataSet[DicomTags.StudyInstanceUid].TryGetString(0, out studyInstanceUid))
                    {
                        skipped = _skippedStudies.Contains(studyInstanceUid);
                        if (!skipped)
                        {
                            DicomSopProcessingResult result = _importer.Import(file, "Importer");
                            if (result.Sussessful)
                            {
                                if (result.Duplicate)
                                {
                                    // was imported but is duplicate
                                }
                                else
                                {
                                    importedSopCount = 1;
                                    Platform.Log(LogLevel.Info, "Imported SOP {0} to {1}", result.SopInstanceUid, _parms.PartitionAE);
                                    ProgressChangedEventArgs progress = new ProgressChangedEventArgs(100, result.SopInstanceUid);

									// Fire the imported event.
									SopImportedEventArgs args = new SopImportedEventArgs();
                                	args.StudyInstanceUid = result.StudyInstanceUid;
                                	args.SeriesInstanceUid = result.SeriesInstanceUid;
                                	args.SopInstanceUid = result.SopInstanceUid;
                                	EventsHelper.Fire(_sopImportedHandlers, this, args);

                                    OnProgressChanged(progress);
                                }
                            }
                            else
                            {
                                if (result.DicomStatus == DicomStatuses.StorageStorageOutOfResources)
                                {
                                    _skippedStudies.Add(result.StudyInstanceUid);
                                    skipped = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Sop does not contains Study Instance Uid tag");
                    }
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                    
                }
                finally
                {
                    if (importedSopCount>0)
                    {
                        fileInfo.Delete();
                    }
                    else if (!isDicomFile)
                    {
                        fileInfo.Delete();
                    }
                    else
                    {
                        //is dicom file but could not be imported.
                        if (!skipped)
                            fileInfo.Delete();
                    }
                }
            }

            return importedSopCount;
        }    

        #endregion
    }
}