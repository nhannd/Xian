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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;
using Ionic.Zip;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Helper class for restoring a study from an <see cref="HsmArchive"/>
	/// </summary>
	public class HsmStudyRestore
	{
		#region Private Members
		private readonly HsmArchive _hsmArchive;
		private ArchiveStudyStorage _archiveStudyStorage;
		private StudyStorageLocation _location;
		private TransferSyntax _syntax;
		private ServerTransferSyntax _serverSyntax;
		private StudyStorage _studyStorage;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hsmArchive">The HsmArchive to work with.</param>
		public HsmStudyRestore(HsmArchive hsmArchive)
		{
			_hsmArchive = hsmArchive;
		}
		#endregion

		/// <summary>
		/// Do the restore.
		/// </summary>
		/// <param name="queueItem">The queue item to restore.</param>
		public void Run(RestoreQueue queueItem)
		{
			try
			{
				// Load up related classes.
				using (IReadContext readContext = _hsmArchive.PersistentStore.OpenReadContext())
				{
					_archiveStudyStorage = ArchiveStudyStorage.Load(readContext, queueItem.ArchiveStudyStorageKey);
					_serverSyntax = ServerTransferSyntax.Load(readContext, _archiveStudyStorage.ServerTransferSyntaxKey);
					_syntax = TransferSyntax.GetTransferSyntax(_serverSyntax.Uid);

					StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
					parms.StudyStorageKey = queueItem.StudyStorageKey;
					IQueryStudyStorageLocation broker = readContext.GetBroker<IQueryStudyStorageLocation>();
					_location = broker.FindOne(parms);
					if (_location == null)
						_studyStorage = StudyStorage.Load(queueItem.StudyStorageKey);
				}

				if (_studyStorage == null)
					Platform.Log(LogLevel.Info, "Starting restore of study: {0}", _location.StudyInstanceUid);
				else
					Platform.Log(LogLevel.Info, "Starting restore of study: {0}", _studyStorage.StudyInstanceUid);

				// If restoring a Nearline study, select a filesystem
				string destinationFolder;
				if (_location == null)
				{
					ServerFilesystemInfo fs = _hsmArchive.Selector.SelectFilesystem();
					if (fs == null)
					{
						DateTime scheduleTime = Platform.Time.AddMinutes(5);
						Platform.Log(LogLevel.Error, "No writeable filesystem for restore, rescheduling restore request to {0}",
						             scheduleTime);
						queueItem.FailureDescription = "No writeable filesystem for restore, rescheduling request.";
						_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Pending, scheduleTime);
						return;
					}
					destinationFolder = Path.Combine(fs.Filesystem.FilesystemPath, _hsmArchive.ServerPartition.PartitionFolder);
				}
				else
					destinationFolder = _location.GetStudyPath();


				// Get the zip file path from the xml data in the ArchiveStudyStorage entry
				// Also store the "StudyFolder" for use below
				string studyFolder = String.Empty;
				string filename = String.Empty;
				string studyInstanceUid = String.Empty;
				XmlElement element = _archiveStudyStorage.ArchiveXml.DocumentElement;
				foreach (XmlElement node in element.ChildNodes)
					if (node.Name.Equals("StudyFolder"))
						studyFolder = node.InnerText;
					else if (node.Name.Equals("Filename"))
						filename = node.InnerText;
					else if (node.Name.Equals("Uid"))
						studyInstanceUid = node.InnerText;

				string zipFile = Path.Combine(_hsmArchive.HsmPath, studyFolder);
				zipFile = Path.Combine(zipFile, studyInstanceUid);
				zipFile = Path.Combine(zipFile, filename);


				// Do a test read of the zip file.  If it succeeds, the file is available, if it 
				// fails, we just set back to pending and recheck.
				try
				{
					FileStream stream = File.OpenRead(zipFile);
					// Read a byte, just in case that makes a difference.
					stream.ReadByte();
					stream.Close();
					stream.Dispose();
				}
				catch (Exception)
				{
					DateTime scheduledTime = Platform.Time.AddSeconds(HsmSettings.Default.ReadFailRescheduleDelaySeconds);
					Platform.Log(LogLevel.Warn, "Study {0} is unreadable, rescheduling restore to {1}", _studyStorage == null ? (_location == null ? string.Empty : _location.StudyInstanceUid) : _studyStorage.StudyInstanceUid,
					             scheduledTime);
					// Just reschedule in "Restoring" state, the file is unreadable.
					_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Restoring,
					                               scheduledTime);
					return;
				}

				if (_location == null)
					RestoreNearlineStudy(queueItem, zipFile, destinationFolder, studyFolder);
				else
					RestoreOnlineStudy(queueItem, zipFile, destinationFolder);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception processing restore request for {0} on archive {1}",
					_studyStorage == null ? (_location == null ? string.Empty : _location.StudyInstanceUid) : _studyStorage.StudyInstanceUid, 
					_hsmArchive.PartitionArchive.Description);
				queueItem.FailureDescription = e.Message;
				_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Failed, Platform.Time);
			}
		}

		public void RestoreNearlineStudy(RestoreQueue queueItem, string zipFile, string destinationFolder, string studyFolder)
		{
            ServerFilesystemInfo fs = _hsmArchive.Selector.SelectFilesystem();
			if (fs == null)
			{
				DateTime scheduleTime = Platform.Time.AddMinutes(5);
				Platform.Log(LogLevel.Error, "No writeable filesystem for restore, rescheduling restore request to {0}", scheduleTime);
				queueItem.FailureDescription = "No writeable filesystem for restore, rescheduling restore request";
				_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Pending, scheduleTime);
				return;
			}

			try
			{
				using (ServerCommandProcessor processor = new ServerCommandProcessor("HSM Restore Offline Study"))
				{
					processor.AddCommand(new CreateDirectoryCommand(destinationFolder));
					destinationFolder = Path.Combine(destinationFolder, studyFolder);
					processor.AddCommand(new CreateDirectoryCommand(destinationFolder));
					destinationFolder = Path.Combine(destinationFolder, _studyStorage.StudyInstanceUid);
					processor.AddCommand(new CreateDirectoryCommand(destinationFolder));
					processor.AddCommand(new ExtractZipCommand(zipFile, destinationFolder));


					// Apply the rules engine.
					ServerActionContext context =
						new ServerActionContext(null, fs.Filesystem.GetKey(), _hsmArchive.PartitionArchive.ServerPartitionKey,
						                        queueItem.StudyStorageKey);
					context.CommandProcessor = processor;
					processor.AddCommand(
						new ApplyRulesCommand(destinationFolder, _studyStorage.StudyInstanceUid, context));

					// Do the actual insert into the DB
					processor.AddCommand(
						new InsertFilesystemStudyStorageCommand(_hsmArchive.PartitionArchive.ServerPartitionKey,
						                                        _studyStorage.StudyInstanceUid,
						                                        studyFolder,
						                                        fs.Filesystem.GetKey(), _syntax));
					if (!processor.Execute())
					{
						Platform.Log(LogLevel.Error, "Unexpected error processing restore request for {0} on archive {1}",
						             _studyStorage.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);
						queueItem.FailureDescription = processor.FailureReason;
						_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Failed, Platform.Time);
					}
					else
					{
						// Unlock the Queue Entry
						using (
							IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
						{
							bool retVal = _hsmArchive.UpdateRestoreQueue(update, queueItem, RestoreQueueStatusEnum.Completed, Platform.Time.AddSeconds(60));
							ILockStudy studyLock = update.GetBroker<ILockStudy>();
							LockStudyParameters parms = new LockStudyParameters();
							parms.StudyStorageKey = queueItem.StudyStorageKey;
							parms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
							retVal = retVal && studyLock.Execute(parms);
							if (!parms.Successful || !retVal)
							{
								string message =
									String.Format("Study {0} on partition {1} failed to unlock.", _studyStorage.StudyInstanceUid,
									              _hsmArchive.ServerPartition.Description);
								Platform.Log(LogLevel.Info, message);
								throw new ApplicationException(message);
							}
							else
								update.Commit();

							Platform.Log(LogLevel.Info, "Successfully restored study: {0} on archive {1}", _studyStorage.StudyInstanceUid,
										 _hsmArchive.PartitionArchive.Description);
						}
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception processing restore request for {0} on archive {1}",
							 _studyStorage.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);
				_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Failed, Platform.Time);
			}
		}

		private void RestoreOnlineStudy(RestoreQueue queueItem, string zipFile, string destinationFolder)
		{
			try
			{
				using (ServerCommandProcessor processor = new ServerCommandProcessor("HSM Restore Online Study"))
				{
					using (ZipFile zip = new ZipFile(zipFile))
					{
						foreach (string file in zip.EntryFileNames)
						{
							processor.AddCommand(new ExtractZipFileAndReplaceCommand(zipFile, file, destinationFolder));
						}
					}
					StudyStatusEnum status;

					if (_syntax.Encapsulated && _syntax.LosslessCompressed)
						status = StudyStatusEnum.OnlineLossless;
					else if (_syntax.Encapsulated && _syntax.LossyCompressed)
						status = StudyStatusEnum.OnlineLossy;
					else
						status = StudyStatusEnum.Online;

					processor.AddCommand(new UpdateStudyStateCommand(_location, status, _serverSyntax));

					if (!processor.Execute())
					{
						Platform.Log(LogLevel.Error, "Unexpected error processing restore request for {0} on archive {1}",
									 _location.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);
						queueItem.FailureDescription = processor.FailureReason;
						_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Failed, Platform.Time);
					}
					else
					{
						// Unlock the Queue Entry and set to complete
						using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
						{
							_hsmArchive.UpdateRestoreQueue(update, queueItem, RestoreQueueStatusEnum.Completed, Platform.Time.AddSeconds(60));
							ILockStudy studyLock = update.GetBroker<ILockStudy>();
							LockStudyParameters parms = new LockStudyParameters();
							parms.StudyStorageKey = queueItem.StudyStorageKey;
							parms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
							bool retVal = studyLock.Execute(parms);
							if (!parms.Successful || !retVal)
							{
								Platform.Log(LogLevel.Info, "Study {0} on partition {1} failed to unlock.", _location.StudyInstanceUid,
											 _hsmArchive.ServerPartition.Description);
							}

							update.Commit();

							Platform.Log(LogLevel.Info, "Successfully restored study: {0} on archive {1}", _location.StudyInstanceUid,
										 _hsmArchive.PartitionArchive.Description);
						}
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception processing restore request for {0} on archive {1}",
							 _location.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);
				queueItem.FailureDescription = e.Message;
				_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Failed, Platform.Time);
			}
		}
	}
}
