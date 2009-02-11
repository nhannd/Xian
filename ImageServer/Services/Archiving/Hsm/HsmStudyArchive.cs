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
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Support class for archiving a specific study with an <see cref="HsmArchive"/>.
	/// </summary>
	public class HsmStudyArchive
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hsmArchive">The HsmArchive to work with.</param>
		public HsmStudyArchive(HsmArchive hsmArchive)
		{
			_hsmArchive = hsmArchive;
		}
		private StudyXml _studyXml;
		private StudyStorageLocation _storageLocation;
		private readonly HsmArchive _hsmArchive;
		private XmlDocument _archiveXml;
		private ServerRulesEngine _rulesEngine;

		/// <summary>
		/// Retrieves the storage location fromthe database for the specified study.
		/// </summary>
		/// <param name="queueItem">The queueItem.</param>
		/// <returns>true if a location was found, false otherwise.</returns>
		public bool GetStudyStorageLocation(ArchiveQueue queueItem)
		{
			return FilesystemMonitor.Instance.GetStudyStorageLocation(queueItem.StudyStorageKey, out _storageLocation);
		}

		/// <summary>
		/// Load the StudyXml file.
		/// </summary>
		/// <param name="studyXmlFile"></param>
		public void LoadStudyXml(string studyXmlFile)
		{
			using (Stream fileStream = FileStreamOpener.OpenForRead(studyXmlFile, FileMode.Open))
			{
				XmlDocument theDoc = new XmlDocument();

				StudyXmlIo.Read(theDoc, fileStream);

				_studyXml = new StudyXml(_storageLocation.StudyInstanceUid);
				_studyXml.SetMemento(theDoc);

				fileStream.Close();
			}
		}


		/// <summary>
		/// Archive the specified <see cref="ArchiveQueue"/> item.
		/// </summary>
		/// <param name="queueItem">The ArchiveQueue item to archive.</param>
		public void Run(ArchiveQueue queueItem)
		{
			try
			{

				_rulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyArchived, _hsmArchive.ServerPartition.GetKey());
				_rulesEngine.Load();

				if (!GetStudyStorageLocation(queueItem))
				{
					Platform.Log(LogLevel.Error,
					             "Unable to find reading study storage location for archival queue request {0}.  Delaying request.",
					             queueItem.Key);
					_hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));
					return;
				}
				Platform.Log(LogLevel.Info, "Starting archival of study {0} on partition {1} to archive {2}", _storageLocation.StudyInstanceUid, _hsmArchive.ServerPartition.Description,
					_hsmArchive.PartitionArchive.Description);

				// First, check to see if we can lock the study, if not just reschedule the queue entry.
				if (!_storageLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle))
				{
					Platform.Log(LogLevel.Info,"Study {0} on partition {1} is currently locked, delaying archival.", _storageLocation.StudyInstanceUid, _hsmArchive.ServerPartition.Description);
					_hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));
					return;
				}

				using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					ILockStudy studyLock = update.GetBroker<ILockStudy>();
					LockStudyParameters parms = new LockStudyParameters();
					parms.StudyStorageKey = queueItem.StudyStorageKey;
					parms.QueueStudyStateEnum = QueueStudyStateEnum.ArchiveScheduled;
					bool retVal = studyLock.Execute(parms);
					if (!parms.Successful || !retVal)
					{
						Platform.Log(LogLevel.Info, "Study {0} on partition {1} is failed to lock, delaying archival.", _storageLocation.StudyInstanceUid, _hsmArchive.ServerPartition.Description);
						_hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));
						return;						
					}
					update.Commit();
				}

				string studyFolder = _storageLocation.GetStudyPath();

				string studyXmlFile = Path.Combine(studyFolder,String.Format("{0}.xml",  _storageLocation.StudyInstanceUid));

				// Load the study Xml file, this is used to generate the list of dicom files to archive.
				LoadStudyXml(studyXmlFile);

				DicomMessage message = LoadMessageFromStudyXml();

				// Use the command processor to do the archival.
                using (ServerCommandProcessor commandProcessor = new ServerCommandProcessor("HSM Archive"))
                {

                    _archiveXml = new XmlDocument();

                    // Create the study date folder
                    string zipFilename = Path.Combine(_hsmArchive.HsmPath, _storageLocation.StudyFolder);
                    commandProcessor.AddCommand(new CreateDirectoryCommand(zipFilename));

                    // Create a folder for the study
                    zipFilename = Path.Combine(zipFilename, _storageLocation.StudyInstanceUid);
                    commandProcessor.AddCommand(new CreateDirectoryCommand(zipFilename));

                    // Save the archive data in the study folder, based on a filename with a date / time stamp
                    string filename = String.Format("{0}.zip", Platform.Time.ToString("yyyy-MM-dd-HHmm"));
                    zipFilename = Path.Combine(zipFilename, filename);


                    // Create the Xml data to store in the ArchiveStudyStorage table telling
                    // where the archived study is located.
                    XmlElement hsmArchiveElement = _archiveXml.CreateElement("HsmArchive");
                    _archiveXml.AppendChild(hsmArchiveElement);
                    XmlElement studyFolderElement = _archiveXml.CreateElement("StudyFolder");
                    hsmArchiveElement.AppendChild(studyFolderElement);
                    studyFolderElement.InnerText = _storageLocation.StudyFolder;
                    XmlElement filenameElement = _archiveXml.CreateElement("Filename");
                    hsmArchiveElement.AppendChild(filenameElement);
                    filenameElement.InnerText = filename;
                    XmlElement studyInstanceUidElement = _archiveXml.CreateElement("Uid");
                    hsmArchiveElement.AppendChild(studyInstanceUidElement);
                    studyInstanceUidElement.InnerText = _storageLocation.StudyInstanceUid;


                    // Create the Zip file
                    commandProcessor.AddCommand(new CreateStudyZipCommand(zipFilename, _studyXml, studyFolder));

                    // Update the database.
                    commandProcessor.AddCommand(new InsertArchiveStudyStorageCommand(queueItem.StudyStorageKey, queueItem.PartitionArchiveKey, queueItem.GetKey(), _storageLocation.ServerTransferSyntaxKey, _archiveXml));

                    // Apply the rules engine.
                    ServerActionContext context = new ServerActionContext(message, _storageLocation.FilesystemKey, _hsmArchive.PartitionArchive.ServerPartitionKey, queueItem.StudyStorageKey);

                    context.CommandProcessor = commandProcessor;

                    _rulesEngine.Execute(context);

                    if (!commandProcessor.Execute())
                    {
                        Platform.Log(LogLevel.Error, "Unexpected failure archiving study");

                        _hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Failed, Platform.Time);

                    }
                    else
                        Platform.Log(LogLevel.Info, "Successfully archived study {0} on {1}", _storageLocation.StudyInstanceUid,
                                     _hsmArchive.PartitionArchive.Description);
                }

				
			}
			catch (Exception e)
			{
                String msg = String.Format("Unexpected exception archiving study: {0} on {1}",
				             _storageLocation.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);

			    Platform.Log(LogLevel.Error, e, msg);

				_hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Failed, Platform.Time);
			}
			finally
			{
				// Unlock the Queue Entry
				using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					ILockStudy studyLock = update.GetBroker<ILockStudy>();
					LockStudyParameters parms = new LockStudyParameters();
					parms.StudyStorageKey = queueItem.StudyStorageKey;
					parms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
					bool retVal = studyLock.Execute(parms);
					if (!parms.Successful || !retVal)
					{
						Platform.Log(LogLevel.Info, "Study {0} on partition {1} is failed to unlock.", _storageLocation.StudyInstanceUid, _hsmArchive.ServerPartition.Description);
					}
					update.Commit();
				}
			}
		}

		/// <summary>
		/// Simple class to load a sample image file from the study.
		/// </summary>
		/// <returns></returns>
		private DicomMessage LoadMessageFromStudyXml()
		{
			DicomMessage defaultMessage = null;
			foreach (SeriesXml seriesXml in _studyXml)
				foreach (InstanceXml instanceXml in seriesXml)
				{
					// Skip non-image objects
					if (instanceXml.SopClass.Equals(SopClass.KeyObjectSelectionDocumentStorage)
						|| instanceXml.SopClass.Equals(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass)
						|| instanceXml.SopClass.Equals(SopClass.BlendingSoftcopyPresentationStateStorageSopClass)
						|| instanceXml.SopClass.Equals(SopClass.ColorSoftcopyPresentationStateStorageSopClass))
					{
						if (defaultMessage == null)
							defaultMessage = new DicomMessage(new DicomAttributeCollection(), instanceXml.Collection);
						continue;
					}

					return new DicomMessage(new DicomAttributeCollection(), instanceXml.Collection);
				}

			return defaultMessage;
		}
	}
}
