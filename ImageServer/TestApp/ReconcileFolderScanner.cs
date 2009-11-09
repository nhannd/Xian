using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.TestApp.SIQDataSetTableAdapters;
using ClearCanvas.ImageServer.TestApp.StudyStorageDataSetTableAdapters;

namespace ClearCanvas.ImageServer.TestApp
{


    public class ResultEntry
    {
        public string Path { get; set; }
        public string StudyInstanceUid { get; set; }
        public bool Skipped { get; set; }
        public bool IsInSIQ { get; set; }
        public bool BackupFilesOnly { get; set; }
        public bool IsEmpty { get; set; }
        public bool StudyIsGone { get; set; }
        public bool StudyWasResent { get; set; }
        public bool NoExplanation { get; set; }


    }

    public class ScanResult
    {
        public int TotalScanned { get { return Results.Count; } }
        public int SkippedCount { get { return Results.FindAll(item => item.Skipped).Count; } }
        public int InSIQCount { get { return Results.FindAll(item => item.IsInSIQ).Count; } }
        public int EmptyCount { get { return Results.FindAll(item => item.IsEmpty).Count; } }
        public int BackupOrTempOnlyCount { get { return Results.FindAll(item => item.BackupFilesOnly).Count; } }
        public int DeletedStudyCount { get { return Results.FindAll(item => item.StudyIsGone).Count; } }
        public int StudyWasResentCount { get { return Results.FindAll(item => item.StudyWasResent).Count; } }
        public int UnidentifiedCount { get { return Results.FindAll(item => item.NoExplanation).Count; } }

        public List<ResultEntry> Results { get; set; }

        public BackgroundTaskProgress Progress { get; internal set; }


        public ScanResult()
        {
            Results = new List<ResultEntry>();
        }
    }

    public class ReconcileFolderScanner
    {
        public string Path { get; set; }

        private IList<StudyDeleteRecord> _deletedStudies;
        private BackgroundTask _worker;
        private int _foldersCount;
        private readonly List<SIQDataSet.StudyIntegrityQueueRow> SIQEntries = new List<SIQDataSet.StudyIntegrityQueueRow>();
        
        public ScanResult ScanResult { get; private set; }

        public event EventHandler ProgressUpdated;
        public event EventHandler Terminated;

        private void LoadSIQEntries()
        {
            using(StudyIntegrityQueueTableAdapter adapter = new StudyIntegrityQueueTableAdapter())
            {
                SIQDataSet.StudyIntegrityQueueDataTable table = new SIQDataSet.StudyIntegrityQueueDataTable();
                adapter.Fill(table);
                foreach (SIQDataSet.StudyIntegrityQueueRow row in table.Rows)
                {
                    SIQEntries.Add(row);
                }
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
            ReconcileFolderScanner owner = context.UserState as ReconcileFolderScanner;
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
                        ProcessDir(subDir);

                    }
                    catch (Exception)
                    {
                        
                    }
                    finally
                    {
                        dirCount++;
                        context.ReportProgress(new BackgroundTaskProgress(dirCount * 100 / _foldersCount, String.Format("Scanning {0}", subDir.FullName)));
                   
                    }
                }


            }

        }

        private void ProcessDir(DirectoryInfo groupDir)
        {
            if (groupDir.LastWriteTimeUtc >= Platform.Time.ToUniversalTime() - TimeSpan.FromMinutes(30))
            {
                // it's new. skip it
                Skip(groupDir);
                return;
            }

            DirectoryInfo[] subDirs = groupDir.GetDirectories();
            FileInfo[] files = groupDir.GetFiles();

            Guid guid = Guid.Empty;
            try
            {
                guid = new Guid(groupDir.Name);
            }
            catch (Exception)
            { }
            bool isInSIQ = !guid.Equals(Guid.Empty) ? FindSIQByGuid(guid) : FindSIQByGroupID(groupDir.Name);

            if (isInSIQ)
            {
                FoundInSIQ(groupDir);
                return;
            }


            if (subDirs.Length == 0 && files.Length == 0)
            {
                FoundEmptyDir(groupDir);

                return;
            }

            // check if all files are backup files
            bool onlyBackupFiles = ContainsOnlyBackupFiles(groupDir);
            if (onlyBackupFiles)
            {
                FoundDirWithBakOrTempFiles(groupDir);
                return;
            }

            string studyUid = FindStudyUid(groupDir);

            if (CheckIfStudyWasDeletedAfterward(groupDir, studyUid))
            {
                FoundDeletedStudy(groupDir, studyUid);
                return;
            }

            if (StudyWasResentAfterward(groupDir, studyUid))
            {
                FoundResentStudy(groupDir, studyUid);
                return;
            }

            ScanResult.Results.Add(new ResultEntry { Path = groupDir.FullName, NoExplanation = true });
        }

        private void FoundResentStudy(DirectoryInfo groupDir, string studyUid)
        {
            ScanResult.Results.Add(new ResultEntry { Path = groupDir.FullName, 
                                                     StudyInstanceUid = studyUid, StudyWasResent = true });
        }

        private bool StudyWasResentAfterward(DirectoryInfo dir, string studyUid)
        {
            DateTime? studyInsertDate = GetStudyInsertTime(studyUid);
            if (studyInsertDate != null)
                return dir.LastWriteTimeUtc < studyInsertDate.Value.ToUniversalTime();
            return false;
        }

        private DateTime? GetStudyInsertTime(string uid)
        {
            StudyStorageDataSet.StudyStorageDataTable results = new StudyStorageDataSet.StudyStorageDataTable();
            
            using(StudyStorageTableAdapter dbAdapter = new StudyStorageTableAdapter())
            {
                dbAdapter.Fill(results, uid);

                if (results.Count > 0)
                    return results[0].InsertTime;
                return null;
            }

            
        }

        private void FoundDeletedStudy(DirectoryInfo groupDir, string studyUid)
        {
            ScanResult.Results.Add(new ResultEntry { Path = groupDir.FullName, StudyInstanceUid = studyUid, StudyIsGone = true });
        }

        private void FoundDirWithBakOrTempFiles(DirectoryInfo groupDir)
        {
            ScanResult.Results.Add(new ResultEntry { Path = groupDir.FullName, BackupFilesOnly = true });
        }

        private void FoundEmptyDir(DirectoryInfo groupDir)
        {
            ScanResult.Results.Add(new ResultEntry { Path = groupDir.FullName, IsEmpty = true });
        }

        private void FoundInSIQ(DirectoryInfo groupDir)
        {
            ScanResult.Results.Add(new ResultEntry { Path = groupDir.FullName, IsInSIQ = true });
        }

        private void Skip(DirectoryInfo groupDir)
        {
            ScanResult.Results.Add(new ResultEntry { Path = groupDir.FullName, Skipped = true });
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

        private static bool ContainsOnlyBackupFiles(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.Name.Contains("bak") && !file.Name.Contains("tmp"))
                    return false;

            }

            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirs)
            {
                if (!ContainsOnlyBackupFiles(subDir))
                    return false;
            }

            return true;
        }

        private bool CheckIfStudyWasDeletedAfterward(DirectoryInfo dir, string suid)
        {
            return (null != CollectionUtils.SelectFirst(
                                _deletedStudies,
                                record => record.StudyInstanceUid == suid &&
                                          record.Timestamp.ToUniversalTime() > dir.LastWriteTimeUtc
                                ));
        }


        private bool FindSIQByGuid(Guid guid)
        {
            return null != SIQEntries.Find(row => row.GUID.Equals(guid));
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
            ScanResult = new ScanResult();
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
            ScanResult.Progress = e.Progress;
            EventsHelper.Fire(ProgressUpdated, this, null);
        }

        public void Stop()
        {
            if (_worker != null)
                _worker.RequestCancel();
        }
    }
}