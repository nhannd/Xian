#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemDelete
{
    public class FilesystemDeleteItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor
    {
        private FilesystemMonitor _monitor;
        private float _bytesToRemove;

        private void QueryCandidates(Model.ServiceLock item)
        {
            IQueryFilesystemQueue query = ReadContext.GetBroker<IQueryFilesystemQueue>();

            FilesystemQueueQueryParameters parms = new FilesystemQueueQueryParameters();

            parms.FilesystemKey = item.FilesystemKey;
            parms.ScheduledTime = Platform.Time;
            parms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.GetEnum("StudyDelete");
            parms.Results = 1000; // grab 1000 rows at a time

            IList<FilesystemQueue> list = query.Execute(parms);

            foreach (FilesystemQueue queueItem in list)
            {
                IQueryStudyStorageLocation studyStorageQuery = ReadContext.GetBroker<IQueryStudyStorageLocation>();

                StudyStorageLocationQueryParameters studyStorageParms = new StudyStorageLocationQueryParameters();
                studyStorageParms.StudyStorageKey = queueItem.StudyStorageKey;

                IList<StudyStorageLocation> storageList = studyStorageQuery.Execute(studyStorageParms);
                StudyStorageLocation location = storageList[0];
                float studySize = CalculateFolderSize(location.GetStudyPath());

                using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IInsertWorkQueueDeleteStudy studyDelete = update.GetBroker<IInsertWorkQueueDeleteStudy>();

                    WorkQueueDeleteStudyInsertParameters insertParms = new WorkQueueDeleteStudyInsertParameters();
                    insertParms.StudyStorageKey = location.GetKey();
                    insertParms.ServerPartitionKey = location.ServerPartitionKey;
                    DateTime expirationTime = Platform.Time.AddSeconds(10);
                    insertParms.ScheduledTime = expirationTime;
                    insertParms.ExpirationTime = expirationTime;

                    if (false == studyDelete.Execute(insertParms))
                    {
                        Platform.Log(LogLevel.Error,"Unexpected problem inserting into WorkQueue");
                    }
                    else
                    {
                        IDeleteFilesystemQueue deleteQueue = update.GetBroker<IDeleteFilesystemQueue>();
                        FilesystemQueueDeleteParameters deleteParms = new FilesystemQueueDeleteParameters();
                        deleteParms.FilesystemQueueKey = item.GetKey();

                        if (false == deleteQueue.Execute(deleteParms))
                        {
                            Platform.Log(LogLevel.Error, "Unexpected problem deleting from FilesystemQueue table");
                        }
                        else
                        {
                            update.Commit();
                            _bytesToRemove -= studySize;
                        }
                    }
                }
                
            }
        }

        public void Process(Model.ServiceLock item)
        {
            _monitor = new FilesystemMonitor();
            _monitor.Load();

            if (_monitor.CheckFilesystemAboveHighWatermark(item.FilesystemKey))
            {
                _bytesToRemove = _monitor.CheckFilesystemBytesToRemove(item.FilesystemKey);

                QueryCandidates(item);

                UnlockServiceLock(item, Platform.Time.AddMinutes(10));
            }
            else if (_monitor.CheckFilesystemAboveLowWatermark(item.FilesystemKey))
            {
                UnlockServiceLock(item, Platform.Time.AddMinutes(5));
            }
            else
                UnlockServiceLock(item, Platform.Time.AddMinutes(10));
        }

        public new void Dispose()
        {
            _monitor.Dispose();
            _monitor = null;

            base.Dispose();
        }
    }
}
