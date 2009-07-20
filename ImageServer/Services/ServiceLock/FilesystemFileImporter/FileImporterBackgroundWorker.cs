#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    /// <summary>
    /// Background worker thread to import dicom files from a directory.
    /// </summary>
    internal class DirectoryImporterBackgroundProcess : BackgroundWorker
    {
        #region Private Fields
        private DateTime _startTimeStamp = Platform.Time;
        private readonly DirectoryImporterParameters _parms;
        private readonly List<string> _skippedStudies = new List<String>();
        private  EventHandler<SopImportedEventArgs> _sopImportedHandlers;
        private SopInstanceImporter _importer;

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
            Thread.CurrentThread.Name = String.Format("Import Files to {0} [{1}]", 
                    _parms.PartitionAE, Thread.CurrentThread.ManagedThreadId);
            
            if (_parms.Directory.Exists)
            {
                
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
                            InitializeImporter();

                            DicomProcessingResult result = _importer.Import(file);
                            if (result.Successful)
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
                            	Platform.Log(LogLevel.Warn, "Failure importing sop: {0}", result.ErrorMessage);
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

        private void InitializeImporter()
        {
            if (_importer==null)
            {
                SopInstanceImporterContext context = new SopInstanceImporterContext(
                                String.Format("{0}_{1}", _parms.PartitionAE, _startTimeStamp.ToString("yyyyMMddhhmmss")),
                                _parms.PartitionAE, _parms.PartitionAE);

                _importer = new SopInstanceImporter(context);
            }
           
        }

        #endregion
    }
}