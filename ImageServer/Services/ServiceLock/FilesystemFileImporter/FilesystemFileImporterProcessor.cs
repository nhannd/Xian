using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    public class FilesystemFileImporterProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
        #region Private Fields
        private readonly List<DirectoryImporterBackgroundProcess> _processes = new List<DirectoryImporterBackgroundProcess>();
        private readonly ManualResetEvent _allCompleted = new ManualResetEvent(false);
        private readonly object _sync = new object();
        #endregion

        #region IServiceLockItemProcessor Members

        public void Process(ClearCanvas.ImageServer.Model.ServiceLock item)
        {
            Platform.Log(LogLevel.Info, "Start importing dicom files from {0}", item.Filesystem.GetAbsolutePath(""));

            foreach(ServerPartition partition in ServerPartitionMonitor.Instance)
            {
                DirectoryImporterParameters p = new DirectoryImporterParameters();
                String path = Path.Combine("Import", partition.PartitionFolder);

                p.Directory = new DirectoryInfo(item.Filesystem.GetAbsolutePath(path));
                p.PartitionAE = partition.AeTitle;
                p.MaxImages = ServiceLockSettings.Default.ImportFileBatchSize;
                p.Filter = "*.dcm";
                DirectoryImporterBackgroundProcess worker = new DirectoryImporterBackgroundProcess(p);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.RunWorkerAsync();
                _processes.Add(worker);
            }

            _allCompleted.WaitOne();
            int delayMinutes = ServiceLockSettings.Default.FilesystemLossyCompressRecheckDelay;
            UnlockServiceLock(item, true, Platform.Time.AddMinutes(delayMinutes));
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_sync)
            {
                _processes.Remove(sender as DirectoryImporterBackgroundProcess);
                if (_processes.Count == 0)
                {
                    _allCompleted.Set();
                }
            }
        }

        protected override void OnCancelling()
        {
            lock (_sync)
            {
                Platform.Log(LogLevel.Info, "Stopping child processes...");
                foreach (DirectoryImporterBackgroundProcess worker in _processes)
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
