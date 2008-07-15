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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ICSharpCode.SharpZipLib.Tar;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	public class HsmStudyArchive
	{
		public HsmStudyArchive(PartitionArchive archive)
		{
			_archive = archive;
		}
		private StudyXml _studyXml;
		private StudyStorageLocation _storageLocation;
		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
		private ServerPartition _partition;
		private readonly PartitionArchive _archive;

		/// <summary>
		/// Retrieves the storage location fromthe database for the specified study.
		/// </summary>
		/// <param name="queueItem">The queueItem.</param>
		/// <returns>true if a location was found, false otherwise.</returns>
		public void GetStudyStorageLocation(ArchiveQueue queueItem)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				_partition = ServerPartition.Load(read, _archive.ServerPartitionKey);

				IQueryStudyStorageLocation procedure = read.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
				parms.StudyStorageKey = queueItem.StudyStorageKey;
				IList<StudyStorageLocation> locationList = procedure.Execute(parms);

				foreach (StudyStorageLocation studyLocation in locationList)
				{
					_storageLocation = studyLocation;
					return;
				}
				return;
			}
		}

		public void UpdateArchiveQueue(ArchiveQueue item, ArchiveQueueStatusEnum status, DateTime scheduledTime)
		{
			using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				UpdateArchiveQueueParameters parms = new UpdateArchiveQueueParameters();
				parms.ArchiveQueueKey = item.GetKey();
				parms.ArchiveQueueStatusEnum = status;
				parms.ScheduledTime = scheduledTime;

				IUpdateArchiveQueue broker = updateContext.GetBroker<IUpdateArchiveQueue>();

				if (broker.Execute(parms))
					updateContext.Commit();
			}
		}

		public void Run(ArchiveQueue queueItem)
		{
			GetStudyStorageLocation(queueItem);

			string studyFolder = _storageLocation.GetStudyPath();

			string studyXmlFile =
				String.Format("{0}{1}{2}.xml", studyFolder, Path.PathSeparator, _storageLocation.StudyInstanceUid);

			using (Stream fileStream = new FileStream(studyXmlFile, FileMode.Open))
			{
				XmlDocument theDoc = new XmlDocument();

				StudyXmlIo.Read(theDoc, fileStream);

				_studyXml.SetMemento(theDoc);

				fileStream.Close();
			}

			string tarFile = String.Format("");

			UpdateArchiveQueue(queueItem,ArchiveQueueStatusEnum.Pending,Platform.Time);
		}

		public void CreateTar(string archiveName, string studyFolder, string studyXml)
		{
			Stream outStream = File.Create(archiveName);


			TarArchive archive = TarArchive.CreateOutputTarArchive(outStream, TarBuffer.DefaultBlockFactor);

			archive.SetKeepOldFiles(false);
			archive.AsciiTranslate = false;

			archive.RootPath = studyFolder;

			//archive.SetUserInfo(this.userId, this.userName, this.groupId, this.groupName);

			// Add the studyXml file

			TarEntry xmlEntry = TarEntry.CreateEntryFromFile(studyXml);
			archive.WriteEntry(xmlEntry, true);


			foreach (SeriesXml seriesXml in _studyXml)
				foreach (InstanceXml instanceXml in seriesXml)
				{
					string filename = string.Format("{0}{1}{2}{3}{4}.dcm", studyFolder,
					                                Path.PathSeparator, seriesXml.SeriesInstanceUid, Path.PathSeparator,
					                                instanceXml.SopInstanceUid);

					TarEntry entry = TarEntry.CreateEntryFromFile(filename);
					archive.WriteEntry(entry, false);
				}
		}
	}
}
