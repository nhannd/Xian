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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemLossyCompress
{
	/// <summary>
	/// Class for processing FilesystemLossyCompress <see cref="ServiceLock"/> entries.
	/// </summary>
	public class FilesystemLossyCompressItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
	{
		#region Private Members
		private int _studiesInserted = 0;
		#endregion

		#region Private Methods
		/// <summary>
		/// Process StudyDelete Candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
		/// </summary>
		/// <param name="candidateList">The list of candidate studies for deleting.</param>
		/// <param name="type">The type of compress.</param>
		private void ProcessCompressCandidates(IEnumerable<FilesystemQueue> candidateList, FilesystemQueueTypeEnum type)
		{
			DateTime scheduledTime = Platform.Time.AddSeconds(10);

			foreach (FilesystemQueue queueItem in candidateList)
			{
				// Check for Shutdown/Cancel
				if (CancelPending) break;

				// First, get the StudyStorage locations for the study, and calculate the disk usage.
				StudyStorageLocation location;
				if (!FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(ReadContext, queueItem.StudyStorageKey, out location))
					continue;

				// Get the disk usage
				StudyXml studyXml = LoadStudyXml(location);

				using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					ILockStudy lockstudy = update.GetBroker<ILockStudy>();
					LockStudyParameters lockParms = new LockStudyParameters();
					lockParms.StudyStorageKey = location.Key;
					lockParms.QueueStudyStateEnum = QueueStudyStateEnum.CompressScheduled;
					if (!lockstudy.Execute(lockParms) || !lockParms.Successful)
					{
						Platform.Log(LogLevel.Warn, "Unable to lock study for inserting Lossy Compress, skipping study ({0}",
									 location.StudyInstanceUid);
						continue;
					}

					scheduledTime = scheduledTime.AddSeconds(3);

					IInsertWorkQueueFromFilesystemQueue workQueueInsert = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

					InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters();
				    insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.CompressStudy;
					insertParms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.LossyCompress;
					insertParms.StudyStorageKey = location.GetKey();
					insertParms.ServerPartitionKey = location.ServerPartitionKey;
					DateTime expirationTime = scheduledTime;
					insertParms.ScheduledTime = expirationTime;
					insertParms.DeleteFilesystemQueue = true;
					insertParms.Data = queueItem.QueueXml;
					insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.CompressStudy;
					insertParms.FilesystemQueueTypeEnum = type;
					
					try
					{
						WorkQueue entry = workQueueInsert.FindOne(insertParms);

						InsertWorkQueueUidFromStudyXml(studyXml,update,entry.GetKey());

						update.Commit();
						_studiesInserted++;
					}
					catch(Exception e) 
					{
						Platform.Log(LogLevel.Error, e, "Unexpected problem inserting 'CompressStudy' record into WorkQueue for Study {0}", location.StudyInstanceUid);
						// throw; -- would cause abort of inserts, go ahead and try everything
					}
				}
			}
		}

		#endregion

        #region Protected Methods
        protected override void OnProcess(Model.ServiceLock item)
		{
			ServerFilesystemInfo fs = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

			Platform.Log(LogLevel.Info,
						 "Starting check for studies to lossy compress on filesystem '{0}'.",
						 fs.Filesystem.Description);

			int delayMinutes = ServiceLockSettings.Default.FilesystemLossyCompressRecheckDelay;

			try
			{
				DateTime deleteTime = Platform.Time;
				FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.LossyCompress;

				IList<FilesystemQueue> list = GetFilesystemQueueCandidates(item, deleteTime, type, false);

				if (list.Count > 0)
				{
					ProcessCompressCandidates(list, type);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing LossyCompress records.");
				delayMinutes = 5;
			}

			DateTime scheduledTime = Platform.Time.AddMinutes(delayMinutes);
			if (_studiesInserted == 0)
				Platform.Log(LogLevel.Info,
							 "No eligible candidates to lossy compress from filesystem '{0}'.  Next scheduled filesystem check {1}",
							 fs.Filesystem.Description, scheduledTime);
			else
				Platform.Log(LogLevel.Info,
							 "Completed inserting lossy compress candidates into WorkQueue: {0}.  {1} studies inserted.  Next scheduled filesystem check {2}",
							 fs.Filesystem.Description, _studiesInserted,
							 scheduledTime);

			UnlockServiceLock(item, true, scheduledTime);
		}

		#endregion
	}
}
