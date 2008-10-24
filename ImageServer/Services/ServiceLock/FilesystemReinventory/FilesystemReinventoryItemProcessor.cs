#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemReinventory
{
    /// <summary>
    /// Class for processing 'FilesystemReinventory' <see cref="Model.ServiceLock"/> rows.
    /// </summary>
    class FilesystemReinventoryItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor
    {
    	#region Private Members
        private IPersistentStore _store;
        private IList<ServerPartition> _partitions;
        #endregion

        #region Private Methods
		private bool GetStudyStorageLocation(ServerEntityKey partitionKey, string studyInstanceUid, out StudyStorageLocation location)
		{
			using (IReadContext context = _store.OpenReadContext())
			{
				IQueryStudyStorageLocation procedure = context.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
				parms.ServerPartitionKey = partitionKey;
				parms.StudyInstanceUid = studyInstanceUid;
				location =  procedure.FindOne(parms);

				return location != null;
			}
		}

        private void ReinventoryFilesystem(Filesystem filesystem, WorkQueuePriorityEnum priority)
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
                        if (false == GetStudyStorageLocation(partition.Key, studyInstanceUid, out location))
                        {
                        	StudyStorage storage;
							if (GetStudyStorage(partition, studyInstanceUid, out storage))
							{
								Platform.Log(LogLevel.Warn, "Study {0} on filesystem partition {1} is offline {2}",studyInstanceUid, partition.Description, studyDir.ToString());
							}
							else
							{
								List<FileInfo> fileList = new List<FileInfo>();
								foreach (DirectoryInfo seriesDir in studyDir.GetDirectories())
								{
									FileInfo[] sopInstanceFiles = seriesDir.GetFiles("*.dcm");

									foreach (FileInfo sopFile in sopInstanceFiles)
										fileList.Add(sopFile);
								}

								if (fileList.Count == 0)
								{
									Platform.Log(LogLevel.Warn, "Found empty study folder: {0}\\{1}", dateDir.Name, studyDir.Name);
									continue;
								}
								
								DicomFile file = null;
								foreach (FileInfo fInfo in fileList)
									try
									{
										file = new DicomFile(fInfo.FullName);
										file.Load(DicomTags.TransferSyntaxUid, DicomReadOptions.DoNotStorePixelDataInDataSet);
										break;
									}
									catch (Exception e)
									{
										Platform.Log(LogLevel.Warn, e, "Unexpected failure loading file: {0}.  Continuing to next file.", fInfo.FullName);
										file = null;
									}

								if (file == null)
								{
									Platform.Log(LogLevel.Warn, "Found directory with no readable files: {0}\\{1}", dateDir.Name, studyDir.Name);
									continue;
								}

								using (IUpdateContext update = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
								{
									IInsertStudyStorage studyInsert = update.GetBroker<IInsertStudyStorage>();
									InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters();
									insertParms.ServerPartitionKey = partition.GetKey();
									insertParms.StudyInstanceUid = studyInstanceUid;
									insertParms.Folder = dateDir.Name;
									insertParms.FilesystemKey = filesystem.GetKey();
									insertParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
									if (file.TransferSyntax.LosslessCompressed)
									{
										insertParms.TransferSyntaxUid = file.TransferSyntax.UidString;
										insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
									}
									else if (file.TransferSyntax.LossyCompressed)
									{
										insertParms.TransferSyntaxUid = file.TransferSyntax.UidString;
										insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
									}
									else
									{
										insertParms.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
										insertParms.StudyStatusEnum = StudyStatusEnum.Online;
									}

									location = studyInsert.FindOne(insertParms);

									// Lock the new study storage for study processing
									ILockStudy lockStudy = update.GetBroker<ILockStudy>();
									LockStudyParameters lockParms = new LockStudyParameters();
									lockParms.StudyStorageKey = location.Key;
									lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ProcessingScheduled;
									if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
										Platform.Log(LogLevel.Error, "Unable to lock study {0} for Study Processing", location.StudyInstanceUid);
									
									update.Commit();
								}

								string studyXml = Path.Combine(location.GetStudyPath(), studyInstanceUid + ".xml");
								if (File.Exists(studyXml))
									File.Delete(studyXml);

								foreach (FileInfo sopFile in fileList)
								{

									String sopInstanceUid = sopFile.Name.Replace(sopFile.Extension, "");

									// Just use a read context here, in hopes of improving 
									// performance.  Every other place in the code should use
									// Update contexts when doing transactions.
									IInsertWorkQueue workQueueInsert =
										ReadContext.GetBroker<IInsertWorkQueue>();

									InsertWorkQueueParameters queueInsertParms =
										new InsertWorkQueueParameters();
									queueInsertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess;
									queueInsertParms.StudyStorageKey = location.GetKey();
									queueInsertParms.ServerPartitionKey = partition.GetKey();
									queueInsertParms.SeriesInstanceUid = sopFile.Directory.Name;
									queueInsertParms.SopInstanceUid = sopInstanceUid;
									queueInsertParms.ScheduledTime = Platform.Time;
									queueInsertParms.ExpirationTime = Platform.Time.AddMinutes(5.0);
									queueInsertParms.WorkQueuePriorityEnum = priority;

									if (workQueueInsert.FindOne(queueInsertParms)==null)
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

		private bool GetStudyStorage(ServerPartition partition, string studyInstanceUid, out StudyStorage storage)
		{
			IStudyStorageEntityBroker broker = ReadContext.GetBroker<IStudyStorageEntityBroker>();
			StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
			criteria.ServerPartitionKey.EqualTo(partition.GetKey());
			criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
			storage = broker.FindOne(criteria);

			if (storage != null)
				return true;

			return false;
		}

        #endregion

        #region Public Methods
        public void Process(Model.ServiceLock item)
        {
        	WorkQueuePriorityEnum priority;

			try
			{
				priority = WorkQueuePriorityEnum.GetEnum(ServiceLockSettings.Default.ReinventoryWorkQueuePriority);
			}
			catch
			{
				priority = WorkQueuePriorityEnum.Medium;
			}

            _store = PersistentStoreRegistry.GetDefaultStore();

            IServerPartitionEntityBroker broker = ReadContext.GetBroker<IServerPartitionEntityBroker>();
            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
        	criteria.AeTitle.SortAsc(0);

            _partitions = broker.Find(criteria);

			ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

            Platform.Log(LogLevel.Info, "Starting reinventory of filesystem: {0}", info.Filesystem.Description);

            ReinventoryFilesystem(info.Filesystem, priority);

            item.ScheduledTime = item.ScheduledTime.AddDays(1);

            UnlockServiceLock(item, false, Platform.Time.AddDays(1));
        }

        
        #endregion

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
