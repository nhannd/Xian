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
        private int importedSopCounter = 0;
        #endregion

        #region IServiceLockItemProcessor Members

        protected override void OnProcess(Model.ServiceLock item)
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

                    if (!parms.Directory.Exists)
                    {
                        parms.Directory.Create();
                    }

                    DirectoryImporterBackgroundProcess process = new DirectoryImporterBackgroundProcess(parms);
                    process.SopImported += delegate { importedSopCounter++; };

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

            UnlockServiceLock(item, true, Platform.Time.AddSeconds(importedSopCounter>0? 5: settings.RecheckDelaySeconds));
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
