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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemReinventory
{
    /// <summary>
    /// Class for processing 'FilesystemReinventory' <see cref="Model.ServiceLock"/> rows.
    /// </summary>
    class FilesystemReinventoryItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor
    {
        #region Private Members
        private FilesystemMonitor _monitor;
        private IPersistentStore _store;
        private IList<ServerPartition> _partitions;
        #endregion

        #region Private Methods
        private void ReinventoryFilesystem(Filesystem filesystem)
        {
            ServerPartition partition;

            DirectoryInfo filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

            foreach(DirectoryInfo partitionDir in filesystemDir.GetDirectories())
            {
                if (GetServerPartition(partitionDir.Name, out partition) == false)
                    continue;

                foreach(DirectoryInfo dateDir in partitionDir.GetDirectories())
                {
                    foreach(DirectoryInfo studyDir in dateDir.GetDirectories())
                    {
                        String studyInstanceUid = studyDir.Name;

                        StudyStorageLocation location;
                        if (false == GetStudyStorageLocation(partition, studyInstanceUid, out location))
                        {
                            IList<StudyStorageLocation> studyLocationList;
                            using (IUpdateContext update = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                            {
                                IInsertStudyStorage studyInsert = update.GetBroker<IInsertStudyStorage>();
                                StudyStorageInsertParameters insertParms = new StudyStorageInsertParameters();
                                insertParms.ServerPartitionKey = partition.GetKey();
                                insertParms.StudyInstanceUid = studyInstanceUid;
                                insertParms.Folder = dateDir.Name;
                                insertParms.FilesystemKey = filesystem.GetKey();

                                studyLocationList = studyInsert.Execute(insertParms);

                                update.Commit();
                            }

                            location = studyLocationList[0];

                            string studyXml = Path.Combine(location.GetStudyPath(), studyInstanceUid + ".xml");
                            if (File.Exists(studyXml))
                                File.Delete(studyXml);

                            foreach (DirectoryInfo seriesDir in studyDir.GetDirectories())
                            {
                                String seriesInstanceUid = seriesDir.Name;
                                FileInfo[] sopInstanceFiles = seriesDir.GetFiles("*.dcm");

                                foreach (FileInfo sopFile in sopInstanceFiles)
                                {
                                    String sopInstanceUid = sopFile.Name.Replace(sopFile.Extension, "");

                                    // Just use a read context here, in hopes of improving 
                                    // performance.  Every other place in the code should use
                                    // Update contexts when doing transactions.
                                    IInsertWorkQueueStudyProcess workQueueInsert =
                                        ReadContext.GetBroker<IInsertWorkQueueStudyProcess>();

                                    WorkQueueStudyProcessInsertParameters queueInsertParms =
                                        new WorkQueueStudyProcessInsertParameters();
                                    queueInsertParms.StudyStorageKey = location.GetKey();
                                    queueInsertParms.ServerPartitionKey = partition.GetKey();
                                    queueInsertParms.SeriesInstanceUid = seriesInstanceUid;
                                    queueInsertParms.SopInstanceUid = sopInstanceUid;
                                    queueInsertParms.ScheduledTime = Platform.Time;
                                    queueInsertParms.ExpirationTime = Platform.Time.AddMinutes(1.0);

                                    if (!workQueueInsert.Execute(queueInsertParms))
                                        Platform.Log(LogLevel.Error,
                                                     "Failure attempting to insert SOP Instance into WorkQueue during Reinventory.");
                                }
                            }
                        }
                    }
                }
            }
        }
        private bool GetServerPartition(string partitionFolderName, out ServerPartition partition)
        {
            foreach (ServerPartition part in _partitions)
            {
                if (part.PartitionFolder == partitionFolderName)
                {
                    partition = part;
                    return true;
                }                
            }

            partition = null;
            return false;
        }
        private bool GetStudyStorageLocation(ServerPartition partition, string studyInstanceUid, out StudyStorageLocation location)
        {
            IQueryStudyStorageLocation procedure = ReadContext.GetBroker<IQueryStudyStorageLocation>();
            StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
            parms.ServerPartitionKey = partition.GetKey();
            parms.StudyInstanceUid = studyInstanceUid;
            IList<StudyStorageLocation> locationList = procedure.Execute(parms);

            foreach (StudyStorageLocation studyLocation in locationList)
            {
                if (_monitor.CheckFilesystemReadable(studyLocation.FilesystemKey))
                {
                    location = studyLocation;
                    return true;
                }
            }
            location = null;
            return false;
        }

        #endregion

        #region Public Methods
        public void Process(Model.ServiceLock item)
        {
            _monitor = new FilesystemMonitor();
            _monitor.Load();
            _store = PersistentStoreRegistry.GetDefaultStore();            
            
            IGetServerPartitions broker = ReadContext.GetBroker<IGetServerPartitions>();
            _partitions = broker.Execute();

            ServerFilesystemInfo info;
            _monitor.Filesystems.TryGetValue(item.FilesystemKey, out info);

            Platform.Log(LogLevel.Info, "Starting reinventory of filesystem: {0}", info.Filesystem.Description);

            ReinventoryFilesystem(info.Filesystem);

            Platform.Log(LogLevel.Info, "Completed reinventory of filesystem: {0}", info.Filesystem.Description);

            item.ScheduledTime = item.ScheduledTime.AddDays(1);

            UnlockServiceLock(item, false, Platform.Time.AddDays(1));
        }
        #endregion
    }
}
