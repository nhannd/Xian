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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Utilities.CleanupReconcile
{
    internal class ScanResultEntry
    {
        public string Path { get; set; }
        public DateTime StudyInsertTime { get; set; }
        public string StudyInstanceUid { get; set; }
        public bool Skipped { get; set; }
        public bool IsInSIQ { get; set; }
        public bool BackupFilesOnly { get; set; }
        public bool IsEmpty { get; set; }
        public bool StudyWasOnceDeleted { get; set; }
        public bool StudyWasResent { get; set; }
        public bool Undetermined { get; set; }
        public DateTime DirectoryLastWriteTime { get; set; }

        public bool StudyNoLongerExists { get; set; }

        public bool IsInWorkQueue { get; set; }
        public StudyStorage Storage { get; set; }
        
    }

    internal class ScanResultSet
    {
        public int TotalScanned { get { return Results.Count; } }
        public int SkippedCount { get { return Results.FindAll(item => item.Skipped).Count; } }
        public int InSIQCount { get { return Results.FindAll(item => item.IsInSIQ).Count; } }
        public int EmptyCount { get { return Results.FindAll(item => item.IsEmpty).Count; } }
        public int BackupOrTempOnlyCount { get { return Results.FindAll(item => item.BackupFilesOnly).Count; } }
        public int DeletedStudyCount { get { return Results.FindAll(item => item.StudyWasOnceDeleted).Count; } }
        public int StudyWasResentCount { get { return Results.FindAll(item => item.StudyWasResent).Count; } }
        public int UnidentifiedCount { get { return Results.FindAll(item => item.Undetermined).Count; } }
        public int StudyDoesNotExistCount { get { return Results.FindAll(item => item.StudyNoLongerExists).Count; } }
        public int StudyIsInWorkQueue { get { return Results.FindAll(item => item.IsInWorkQueue).Count; } }

        public List<ScanResultEntry> Results { get; set; }

        public BackgroundTaskProgress Progress { get; internal set; }


        public ScanResultSet()
        {
            Results = new List<ScanResultEntry>();
        }
    }

    internal class FolderScanner
    {
        public string Path { get; set; }

        private IList<StudyDeleteRecord> _deletedStudies;
        private BackgroundTask _worker;
        private int _foldersCount;
        private List<StudyIntegrityQueue> SIQEntries = new List<StudyIntegrityQueue>();
        
        public ScanResultSet ScanResultSet { get; private set; }

        public event EventHandler ProgressUpdated;
        public event EventHandler Terminated;

        private void LoadSIQEntries()
        {
            using(IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyIntegrityQueueEntityBroker broker = ctx.GetBroker<IStudyIntegrityQueueEntityBroker>();
                SIQEntries = new List<StudyIntegrityQueue>(broker.Find(new StudyIntegrityQueueSelectCriteria()));
            }
           
        }

        private void LoadDeletedStudies()
        {
            using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyDeleteRecordEntityBroker broker = context.GetBroker<IStudyDeleteRecordEntityBroker>();
                _deletedStudies = broker.Find(new StudyDeleteRecordSelectCriteria());
            }
        }
        
        private void StartScaning(IBackgroundTaskContext context)
        {
            FolderScanner owner = context.UserState as FolderScanner;
            DirectoryInfo dir = new DirectoryInfo(owner.Path);
            int dirCount = 0;
            if (dir.Exists)
            {
                context.ReportProgress(new BackgroundTaskProgress(0, "Starting.."));
                
                DirectoryInfo[] subDirs = dir.GetDirectories();
                _foldersCount = subDirs.Length;
                
                foreach (DirectoryInfo subDir in subDirs)
                {
                    if (context.CancelRequested)
                    {
                        break;
                    }
                  
                    try
                    {
                        var result = ProcessDir(subDir);
                        ScanResultSet.Results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        dirCount++;
                        context.ReportProgress(new BackgroundTaskProgress(dirCount * 100 / _foldersCount, String.Format("Scanning {0}", subDir.FullName)));
                   
                    }
                }


            }

        }

        private ScanResultEntry ProcessDir(DirectoryInfo groupDir)
        {

            var result = new ScanResultEntry
                             {
                                 Path = groupDir.FullName,
                                 DirectoryLastWriteTime = groupDir.LastWriteTimeUtc
                             };

            if (groupDir.LastWriteTimeUtc >= Platform.Time.ToUniversalTime() - TimeSpan.FromMinutes(30))
            {
                return new ScanResultEntry { Path = groupDir.FullName, Skipped = true };
            }

            if (CheckInSIQ(groupDir, result))
                return result;

            
            if (CheckIfFolderIsEmpty(groupDir, result))
                return result;

            // check if all files are backup files
            if (ContainsOnlyBackupFiles(groupDir, result))
                return result;

            result.StudyInstanceUid = FindStudyUid(groupDir);
            
            if (CheckIfStudyNoLongerExists(groupDir, result))
            {
                return result;
            }

            Debug.Assert(result.Storage != null);
            if (CheckIfStudyIsInWorkQueue(groupDir, result))
                return result;
            
            if (CheckIfStudyWasDeletedAfterward(groupDir, result))
            {
                return result;
            }

            if (CheckIfStudyWasInsertedAfter(groupDir, result))
                return result;

            return result;
        }

        private bool CheckIfStudyIsInWorkQueue(DirectoryInfo dir, ScanResultEntry scanResult)
        {

            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IWorkQueueEntityBroker broker = ctx.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(scanResult.Storage.Key);
                var list = broker.Find(criteria);
                scanResult.IsInWorkQueue = list != null && list.Count > 0;
            } 
            
            return scanResult.IsInWorkQueue;
        }

        private bool CheckIfFolderIsEmpty(DirectoryInfo dir, ScanResultEntry scanResult)
        {
            DirectoryInfo[] subDirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            if (subDirs.Length == 0 && files.Length == 0)
            {
                scanResult.IsEmpty = true;
                return true;
            }

            if (files.Length > 0)
            {
                scanResult.IsEmpty = false;
                return false;
            }

            foreach(DirectoryInfo subDir in subDirs)
            {
                if (!CheckIfFolderIsEmpty(subDir, scanResult))
                {
                    scanResult.IsEmpty = false;
                    return false;
                }
            }

            scanResult.IsEmpty = true;
            return true;
        }

        private bool CheckIfStudyWasInsertedAfter(DirectoryInfo dir, ScanResultEntry scanResult)
        {
            scanResult.StudyWasResent = dir.LastWriteTimeUtc < scanResult.StudyInsertTime.ToUniversalTime();

            return scanResult.StudyWasResent;
        }

        private bool CheckIfStudyNoLongerExists(DirectoryInfo dir, ScanResultEntry scanResult)
        {
            StudyStorage storage = FindStudyStorage(scanResult.StudyInstanceUid);

            scanResult.StudyNoLongerExists = storage == null;
            if (storage != null)
            {
                scanResult.StudyInsertTime = storage.InsertTime;
                scanResult.Storage = storage;
            }

            return scanResult.StudyNoLongerExists;
        }

        private bool CheckInSIQ(DirectoryInfo groupDir, ScanResultEntry scanResult)
        {
            Guid guid = Guid.Empty;
            try
            {
                guid = new Guid(groupDir.Name);
            }
            catch (Exception)
            { }

            scanResult.IsInSIQ = !guid.Equals(Guid.Empty) ? FindSIQByGuid(guid) : FindSIQByGroupID(groupDir.Name);

            return scanResult.IsInSIQ;
        }

        private StudyStorage FindStudyStorage(string uid)
        {
            using(IReadContext ctx= PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyStorageEntityBroker broker = ctx.GetBroker<IStudyStorageEntityBroker>();
                StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
                criteria.StudyInstanceUid.EqualTo(uid);
                return broker.FindOne(criteria);
            }
        }

        private static string FindStudyUid(DirectoryInfo dir)
        {
            FileInfo f = GetFirstFile(dir);

            if (f == null)
                throw new Exception(String.Format("{0}: Cannot find the study instance uid without any dicom files", dir.FullName));

            DicomFile file = new DicomFile(f.FullName);
            file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

            return file.DataSet[DicomTags.StudyInstanceUid].ToString();

        }

        private static bool ContainsOnlyBackupFiles(DirectoryInfo dir, ScanResultEntry scanResult)
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.Name.Contains("bak") && !file.Name.Contains("tmp"))
                {
                    scanResult.BackupFilesOnly = false;
                    return false;
                }

            }

            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirs)
            {
                if (!ContainsOnlyBackupFiles(subDir, scanResult))
                {
                    scanResult.BackupFilesOnly = false; 
                    return false;
                }
            }

            scanResult.BackupFilesOnly = true; 
            return true;
        }

        private bool CheckIfStudyWasDeletedAfterward(DirectoryInfo dir, ScanResultEntry scanResult)
        {
            scanResult.StudyWasOnceDeleted = (null != CollectionUtils.SelectFirst(_deletedStudies,
                                                                                  record => record.StudyInstanceUid == scanResult.StudyInstanceUid && record.Timestamp.ToUniversalTime() > dir.LastWriteTimeUtc
                                                          ));
            return scanResult.StudyWasOnceDeleted;
        }


        private bool FindSIQByGuid(Guid guid)
        {
            return null != SIQEntries.Find(entry => entry.Key.Key.Equals(guid));
        }

        private static FileInfo GetFirstFile(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles();
            if (files.Length > 0)
                return files[0];

            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirs)
            {
                FileInfo f = GetFirstFile(subDir);
                if (f != null)
                    return f;
            }

            return null;
        }

        private bool FindSIQByGroupID(string group)
        {
            return null != SIQEntries.Find(row => row.GroupID != null && row.GroupID.Equals(group));
        }

        public void StartAsync()
        {
            ScanResultSet = new ScanResultSet();
            LoadDeletedStudies();
            LoadSIQEntries(); 
            
            _worker = new BackgroundTask(StartScaning, true, this);
            _worker.ProgressUpdated += WorkerProgressUpdated;
            _worker.Terminated += WorkerTerminated;
            _worker.Run();

        }

        void WorkerTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            EventsHelper.Fire(Terminated, this, null);
        }

        void WorkerProgressUpdated(object sender, BackgroundTaskProgressEventArgs e)
        {
            ScanResultSet.Progress = e.Progress;
            EventsHelper.Fire(ProgressUpdated, this, null);
        }

        public void Stop()
        {
            if (_worker != null)
                _worker.RequestCancel();
        }
    }
}