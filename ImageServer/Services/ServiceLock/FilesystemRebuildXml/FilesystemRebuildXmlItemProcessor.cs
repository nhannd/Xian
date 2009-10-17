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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Rebuild;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemRebuildXml
{
	/// <summary>
	/// Class for processing 'FilesystemRebuildXml' <see cref="Model.ServiceLock"/> rows.
	/// </summary>
	public class FilesystemRebuildXmlItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
	{
		#region Private Members

		private IList<ServerPartition> _partitions;
		#endregion

		#region Private Methods
		/// <summary>
		/// Traverse the filesystem directories for studies to rebuild the XML for.
		/// </summary>
		/// <param name="filesystem"></param>
		private void TraverseFilesystemStudies(Filesystem filesystem)
		{
			List<StudyStorageLocation> lockFailures = new List<StudyStorageLocation>();
			ServerPartition partition;

			DirectoryInfo filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

			foreach (DirectoryInfo partitionDir in filesystemDir.GetDirectories())
			{
				if (GetServerPartition(partitionDir.Name, out partition) == false)
					continue;

				foreach (DirectoryInfo dateDir in partitionDir.GetDirectories())
				{
					if (dateDir.FullName.EndsWith("Deleted")
					    || dateDir.FullName.EndsWith("Reconcile"))
						continue;

					foreach (DirectoryInfo studyDir in dateDir.GetDirectories())
					{
						// Check for Cancel message
						if (CancelPending) return;

						String studyInstanceUid = studyDir.Name;

						StudyStorageLocation location;
						if (false == GetStudyStorageLocation(partition.Key, studyInstanceUid, out location))
						{
							List<FileInfo> fileList = LoadSopFiles(studyDir, true);
							if (fileList.Count == 0)
							{
								Platform.Log(LogLevel.Warn, "Found empty study folder: {0}\\{1}", dateDir.Name, studyDir.Name);
								continue;
							}

							DicomFile file = LoadFileFromList(fileList);
							if (file == null)
							{
								Platform.Log(LogLevel.Warn, "Found directory with no readable files: {0}\\{1}", dateDir.Name, studyDir.Name);
								continue;
							}

							// Do a second check, using the study instance uid from a file in the directory.
							// had an issue with trailing periods on uids causing us to not find the 
							// study storage, and insert a new record into the database.
							studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
							if (!GetStudyStorageLocation(partition.Key, studyInstanceUid, out location))
							{
								StudyStorage storage;
								if (GetStudyStorage(partition, studyInstanceUid, out storage))
								{
									Platform.Log(LogLevel.Warn, "Study {0} on filesystem partition {1} is offline {2}", studyInstanceUid,
									             partition.Description, studyDir.ToString());
								}
								else
									Platform.Log(LogLevel.Info, "Found study directory not in the database, ignoring: {0}", studyDir.ToString());

								continue;
							}
						}
						try
						{
							if (!location.AcquireLock())
							{
								Platform.Log(LogLevel.Warn, "Unable to lock study: {0}, delaying rebuild", location.StudyInstanceUid);
								lockFailures.Add(location);
								continue;
							}

							StudyXmlRebuilder rebuilder = new StudyXmlRebuilder(location);
							rebuilder.RebuildXml();

							location.ReleaseLock();
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e, "Unexpected exception when rebuilding study xml for study: {0}",
							             location.StudyInstanceUid);
							lockFailures.Add(location);
						}
					}


					// Cleanup the parent date directory, if its empty
					DirectoryUtility.DeleteIfEmpty(dateDir.FullName);
				}
			}

			// Re-do all studies that failed locks one time
			foreach (StudyStorageLocation location in lockFailures)
			{
				try
				{
					if (!location.AcquireLock())
					{
						Platform.Log(LogLevel.Warn, "Unable to lock study: {0}, skipping rebuild", location.StudyInstanceUid);
						continue;
					}

					StudyXmlRebuilder rebuilder = new StudyXmlRebuilder(location);
					rebuilder.RebuildXml();

					location.ReleaseLock();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception on retry when rebuilding study xml for study: {0}",
										 location.StudyInstanceUid);
				}
			}
		}

		/// <summary>
		/// Get the server partition
		/// </summary>
		/// <param name="partitionFolderName"></param>
		/// <param name="partition"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Get the study storage for a potentially nearline/offline study
		/// </summary>
		/// <param name="partition"></param>
		/// <param name="studyInstanceUid"></param>
		/// <param name="storage"></param>
		/// <returns></returns>
		private bool GetStudyStorage(ServerPartition partition, string studyInstanceUid, out StudyStorage storage)
		{
			IStudyStorageEntityBroker broker = ReadContext.GetBroker<IStudyStorageEntityBroker>();
			StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
			criteria.ServerPartitionKey.EqualTo(partition.Key);
			criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
			storage = broker.FindOne(criteria);

			if (storage != null)
				return true;

			return false;
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Process the <see cref="ServiceLock"/> item.
		/// </summary>
		/// <param name="item"></param>
        protected override void OnProcess(Model.ServiceLock item)
		{
			PersistentStoreRegistry.GetDefaultStore();

			IServerPartitionEntityBroker broker = ReadContext.GetBroker<IServerPartitionEntityBroker>();
			ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
			criteria.AeTitle.SortAsc(0);

			_partitions = broker.Find(criteria);

			ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

			Platform.Log(LogLevel.Info, "Starting rebuilding of Study XML files for filesystem: {0}", info.Filesystem.Description);

			TraverseFilesystemStudies(info.Filesystem);

			item.ScheduledTime = item.ScheduledTime.AddDays(1);

			if (CancelPending)
			{
				Platform.Log(LogLevel.Info,
							 "FilesystemRebuildXml of {0} has been canceled, rescheduling.  Note that the entire Filesystem will be rebuilt again.",
							 info.Filesystem.Description);
				UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
			}
			else
				UnlockServiceLock(item, false, Platform.Time.AddDays(1));

			Platform.Log(LogLevel.Info, "Completed rebuilding of the Study XML files for filesystem: {0}", info.Filesystem.Description);
		}


		#endregion

		public new void Dispose()
		{
			base.Dispose();
		}
	}
}
