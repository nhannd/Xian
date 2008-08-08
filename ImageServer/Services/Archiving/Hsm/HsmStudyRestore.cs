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
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Helper class for restoring a study from an <see cref="HsmArchive"/>
	/// </summary>
	public class HsmStudyRestore
	{
		#region Private Members
		private readonly HsmArchive _hsmArchive;
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
			Filesystem fs = _hsmArchive.Selector.SelectFilesystem();
			if (fs == null)
			{
				DateTime scheduleTime = Platform.Time.AddMinutes(5);
				Platform.Log(LogLevel.Error, "No writeable filesystem for restore, rescheduling restore request to {0}", scheduleTime);
				_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Pending, scheduleTime);
				return;
			}
			
			ArchiveStudyStorage archiveStudyStorage = ArchiveStudyStorage.Load(queueItem.ArchiveStudyStorageKey);
			StudyStorage studyStorage = StudyStorage.Load(queueItem.StudyStorageKey);
			ServerTransferSyntax serverSyntax = ServerTransferSyntax.Load(archiveStudyStorage.ServerTransferSyntaxKey);
			TransferSyntax syntax = TransferSyntax.GetTransferSyntax(serverSyntax.Uid);

			string studyFolder = String.Empty;
			string filename = String.Empty;

			XmlElement element = archiveStudyStorage.ArchiveXml.DocumentElement;
			foreach (XmlElement node in element.ChildNodes)
				if (node.Name.Equals("StudyFolder"))
					studyFolder = node.InnerText;
				else if (node.Name.Equals("Filename"))
					filename = node.InnerText;

			string zipFile = Path.Combine(_hsmArchive.HsmPath, studyFolder);
			zipFile = Path.Combine(zipFile, studyStorage.StudyInstanceUid);
			zipFile = Path.Combine(zipFile, filename);

			try
			{
				FileStream stream = File.OpenRead(zipFile);
				stream.Close();
				stream.Dispose();
			}
			catch (Exception)
			{
				// Just reschedule, the file is unreadable.
				_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Pending,
				                               Platform.Time.AddSeconds(HsmSettings.Default.ReadFailRescheduleDelaySeconds));
				return;
			}

			try
			{
				ServerCommandProcessor processor = new ServerCommandProcessor("HSM Restore Study");

				string destinationFolder = Path.Combine(fs.FilesystemPath, _hsmArchive.ServerPartition.PartitionFolder);
				processor.AddCommand(new CreateDirectoryCommand(destinationFolder));
				destinationFolder = Path.Combine(destinationFolder, studyFolder);
				processor.AddCommand(new CreateDirectoryCommand(destinationFolder));
				destinationFolder = Path.Combine(destinationFolder, studyStorage.StudyInstanceUid);
				processor.AddCommand(new CreateDirectoryCommand(destinationFolder));

				processor.AddCommand(new ExtractZipCommand(zipFile, destinationFolder));

				processor.AddCommand(
					new InsertFilesystemStudyStorageCommand(studyStorage.ServerPartitionKey, studyStorage.StudyInstanceUid, studyFolder,
					                                        fs.GetKey(), syntax));

				// Apply the rules engine.
				ServerActionContext context = new ServerActionContext(null, fs.GetKey(), _hsmArchive.PartitionArchive.ServerPartitionKey, queueItem.StudyStorageKey);

				context.CommandProcessor = processor;


				if (!processor.Execute())
				{
					Platform.Log(LogLevel.Error, "Unexpected error processing restore request for {0} on archive {1}",
								 studyStorage.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);
					_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Failed, Platform.Time);
				}
				else
				{
					_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Completed, Platform.Time.AddSeconds(60));
					Platform.Log(LogLevel.Info, "Successfully restored study: {0} on archive {1}", studyStorage.StudyInstanceUid,_hsmArchive.PartitionArchive.Description);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception processing restore request for {0} on archive {1}",
				             studyStorage.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);
				_hsmArchive.UpdateRestoreQueue(queueItem, RestoreQueueStatusEnum.Failed, Platform.Time);
			}
		}
	}
}
