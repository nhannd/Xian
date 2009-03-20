using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{

    public class FilesystemFileImporterProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
        #region Private Fields
        private const String DIRECTORY_SUFFIX = "Incoming";
        private readonly Queue<DirectoryImporterBackgroundProcess> _queue = new Queue<DirectoryImporterBackgroundProcess>();
        private readonly List<DirectoryImporterBackgroundProcess> _inprogress = new List<DirectoryImporterBackgroundProcess>();
        private readonly ManualResetEvent _allCompleted = new ManualResetEvent(false);
        private readonly object _sync = new object();
        #endregion

        #region IServiceLockItemProcessor Members

        public void Process(Model.ServiceLock item)
        {
            DirectoryImportSettings settings = DirectoryImportSettings.Default;

            ServerFilesystemInfo filesystem = EnsureFilesystemIsValid(item);
            if (filesystem != null)
            {
                Platform.Log(LogLevel.Debug, "Start importing dicom files from {0}", filesystem.Filesystem.FilesystemPath);

                foreach (ServerPartition partition in ServerPartitionMonitor.Instance)
                {
                    DirectoryImporterParameters parms = new DirectoryImporterParameters();
                    String incomingFolder = String.Format("{0}_{1}", partition.AeTitle, DIRECTORY_SUFFIX);

                    parms.Directory = new DirectoryInfo(filesystem.Filesystem.GetAbsolutePath(incomingFolder));
                    parms.PartitionAE = partition.AeTitle;
                    parms.MaxImages = settings.MaxBatchSize;
                    parms.Delay = settings.ImageDelay;
                    parms.Filter = "*.*";
                    DirectoryImporterBackgroundProcess process = new DirectoryImporterBackgroundProcess(parms);
                    
                    _queue.Enqueue(process);
                }

                // start the processes.
                for (int n = 0; n < settings.MaxConcurrency && n < _queue.Count; n++)
                {
                    LaunchNextBackgroundProcess();
                }

                _allCompleted.WaitOne();

                if (CancelPending)
                    Platform.Log(LogLevel.Info, "All import processes have completed gracefully.");
            }
            UnlockServiceLock(item, true, Platform.Time.AddSeconds(settings.RecheckDelaySeconds));
        }

        #endregion

        #region Private Methods
        private ServerFilesystemInfo EnsureFilesystemIsValid(Model.ServiceLock item)
        {
            ServerFilesystemInfo filesystem = null;
            if (item.FilesystemKey != null)
            {
                filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);
                if (filesystem == null)
                {
                    Platform.Log(LogLevel.Warn, "Filesystem for incoming folders is no longer valid.  Assigning new filesystem.");
                    item.FilesystemKey = null;
                    UpdateFilesystemKey(item);
                }
            }


            if (filesystem == null)
            {
                filesystem = SelectFilesystem();

                if (filesystem == null)
                {
                    UnlockServiceLock(item, true, Platform.Time.AddHours(2));
                }
                else
                {
                    item.FilesystemKey = filesystem.Filesystem.Key;
                    UpdateFilesystemKey(item);
                }
            }
            return filesystem;
        }

        private ServerFilesystemInfo SelectFilesystem()
        {
            IEnumerable<ServerFilesystemInfo> filesystems = FilesystemMonitor.Instance.GetFilesystems();
            IList<ServerFilesystemInfo> sortedFilesystems = CollectionUtils.Sort(filesystems, FilesystemSorter.SortByFreeSpace);

            if (sortedFilesystems == null || sortedFilesystems.Count == 0)
                return null;
            else
                return sortedFilesystems[0];
        }

        private void OnBackgroundProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_sync)
            {
                _inprogress.Remove(sender as DirectoryImporterBackgroundProcess);

                if (!CancelPending && _queue.Count>0)
                {
                    if (_queue.Peek() != null)
                    {
                        LaunchNextBackgroundProcess();
                    }
                }
                
                if (_inprogress.Count==0)
                {
                    _allCompleted.Set();
                }
            }
        }

        private void LaunchNextBackgroundProcess()
        {
            DirectoryImporterBackgroundProcess process = _queue.Dequeue();
            if (process!=null)
            {
                process.RunWorkerCompleted += OnBackgroundProcessCompleted;
                _inprogress.Add(process); 
                process.RunWorkerAsync();
            }
        }

        private static void UpdateFilesystemKey(Model.ServiceLock item)
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ServiceLockUpdateColumns columns = new ServiceLockUpdateColumns();
                columns.FilesystemKey = item.FilesystemKey;

                IServiceLockEntityBroker broker = ctx.GetBroker<IServiceLockEntityBroker>();
                broker.Update(item.Key, columns);
                ctx.Commit();
            }
        }

        #endregion

        #region Overriden Protected Methods

        protected override void OnCancelling()
        {
            lock (_sync)
            {
                Platform.Log(LogLevel.Info, "Signalling child processes to stop...");
                foreach (DirectoryImporterBackgroundProcess worker in _inprogress)
                {
                    if (worker.WorkerSupportsCancellation)
                    {
                        worker.CancelAsync();
                    }
                }
            }
            
        }

        #endregion
    }
}
