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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    /// <summary>
    /// Class for reprocessing the rules engine for studies on a filesystem.
    /// </summary>
    public class FilesystemStudyProcessItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
        #region Private Members
        private readonly FilesystemReprocessStatistics _stats = new FilesystemReprocessStatistics();
        private readonly Dictionary<ServerPartition, ServerRulesEngine> _engines = new Dictionary<ServerPartition, ServerRulesEngine>();
		private readonly Dictionary<ServerPartition, ServerRulesEngine> _postArchivalEngines = new Dictionary<ServerPartition, ServerRulesEngine>();
		#endregion

        #region Private Methods

        /// <summary>
        /// Load the first instance from the first series of the StudyXml file for a study.
        /// </summary>
        /// <param name="location">The storage location of the study.</param>
        /// <returns></returns>
        private static DicomFile LoadInstance(StudyStorageLocation location)
        {
            string studyXml = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

            if (!File.Exists(studyXml))
            {
                return null;
            }

			FileStream stream = FileStreamOpener.OpenForRead(studyXml, FileMode.Open);
            XmlDocument theDoc = new XmlDocument();
            StudyXmlIo.Read(theDoc, stream);
            stream.Close();
            stream.Dispose();
            StudyXml xml = new StudyXml();
            xml.SetMemento(theDoc);           
            
            IEnumerator<SeriesXml> seriesEnumerator = xml.GetEnumerator();
            if (seriesEnumerator.MoveNext())
            {
                SeriesXml seriesXml = seriesEnumerator.Current;
                IEnumerator<InstanceXml> instanceEnumerator = seriesXml.GetEnumerator();
                if (instanceEnumerator.MoveNext())
                {
                    InstanceXml instance = instanceEnumerator.Current;
					DicomFile file = new DicomFile("file.dcm",new DicomAttributeCollection(), instance.Collection)
					                 	{TransferSyntax = instance.TransferSyntax};
                	return file;
                }
            }

            return null;
        }

        /// <summary>
        /// Reprocess a specific study.
        /// </summary>
        /// <param name="partition">The ServerPartition the study is on.</param>
        /// <param name="location">The storage location of the study to process.</param>
        /// <param name="engine">The rules engine to use when processing the study.</param>
        /// <param name="postArchivalEngine">The rules engine used for studies that have been archived.</param>
        private static void ProcessStudy(ServerPartition partition, StudyStorageLocation location, ServerRulesEngine engine, ServerRulesEngine postArchivalEngine)
        {
            if (!location.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle) || !location.AcquireWriteLock())
            {
                Platform.Log(LogLevel.Error, "Unable to lock study {0}. The study is being processed. (Queue State: {1})", location.StudyInstanceUid,location.QueueStudyStateEnum.Description); 
            }
            else
            {
                try
                {
                    DicomFile msg = LoadInstance(location);
                    if (msg == null)
                    {
                        Platform.Log(LogLevel.Error, "Unable to load file for study {0}", location.StudyInstanceUid);
                        return;
                    }

                	bool archiveQueueExists;
					bool archiveStudyStorageExists;
                	bool filesystemDeleteExists;
					using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
					{
						// Check for existing archive queue entries
						IArchiveQueueEntityBroker archiveQueueBroker = read.GetBroker<IArchiveQueueEntityBroker>();
						ArchiveQueueSelectCriteria archiveQueueCriteria = new ArchiveQueueSelectCriteria();
						archiveQueueCriteria.StudyStorageKey.EqualTo(location.Key);
						archiveQueueExists = archiveQueueBroker.Count(archiveQueueCriteria) > 0;


						IArchiveStudyStorageEntityBroker archiveStorageBroker = read.GetBroker<IArchiveStudyStorageEntityBroker>();
						ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
						archiveStudyStorageCriteria.StudyStorageKey.EqualTo(location.Key);
						archiveStudyStorageExists = archiveStorageBroker.Count(archiveStudyStorageCriteria) > 0;

						IFilesystemQueueEntityBroker filesystemQueueBroker = read.GetBroker<IFilesystemQueueEntityBroker>();
						FilesystemQueueSelectCriteria filesystemQueueCriteria = new FilesystemQueueSelectCriteria();
						filesystemQueueCriteria.StudyStorageKey.EqualTo(location.Key);
						filesystemQueueCriteria.FilesystemQueueTypeEnum.EqualTo(FilesystemQueueTypeEnum.DeleteStudy);
						filesystemDeleteExists = filesystemQueueBroker.Count(filesystemQueueCriteria) > 0;
					}
                    
                    ServerActionContext context = new ServerActionContext(msg, location.FilesystemKey, partition, location.GetKey());
                    using (context.CommandProcessor = new ServerCommandProcessor("Study Rule Processor"))
                    {
						// Check if the Study has been archived 
						if (archiveStudyStorageExists && !archiveQueueExists && !filesystemDeleteExists)
						{
							// Add a command to delete the current filesystemQueue entries, so that they can 
							// be reinserted by the rules engine.
							context.CommandProcessor.AddCommand(new DeleteFilesystemQueueCommand(location.GetKey(), ServerRuleApplyTimeEnum.StudyArchived));

							// How to deal with exiting FilesystemQueue entries is problematic here.  If the study
							// has been migrated off tier 1, we probably don't want to modify the tier migration 
							// entries.  Compression entries may have been entered when the Study was initially 
							// processed, we don't want to delete them, because they might still be valid.  
							// We just re-run the rules engine at this point, and delete only the StudyPurge entries,
							// since those we know at least would only be applied for archived studies.
							postArchivalEngine.Execute(context);
						}
						else
						{
							// Add a command to delete the current filesystemQueue entries, so that they can 
							// be reinserted by the rules engine.
							context.CommandProcessor.AddCommand(new DeleteFilesystemQueueCommand(location.GetKey(),ServerRuleApplyTimeEnum.StudyProcessed));

							// Execute the rules engine, insert commands to update the database into the command processor.
							engine.Execute(context);

							// Re-do insert into the archive queue.
							// Note: the stored procedure will update the archive entry if it already exists
							context.CommandProcessor.AddCommand(
								new InsertArchiveQueueCommand(location.ServerPartitionKey, location.GetKey()));
						}

                    	// Do the actual database updates.
                        if (false == context.CommandProcessor.Execute())
                        {
                            Platform.Log(LogLevel.Error, "Unexpected failure processing Study level rules for study {0}", location.StudyInstanceUid);
                        }

						// Log the FilesystemQueue related entries
						location.LogFilesystemQueue();
                    }
                }
                finally
                {
                    location.ReleaseWriteLock();
                }
            }

            
        }

        /// <summary>
        /// Reprocess a file systems
        /// </summary>
        /// <param name="filesystem"></param>
        private void ReprocessFilesystem(Filesystem filesystem)
        {
            DirectoryInfo filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

            foreach (DirectoryInfo partitionDir in filesystemDir.GetDirectories())
            {
                ServerPartition partition;
                if (GetServerPartition(partitionDir.Name, out partition) == false)
                {
					if (!partitionDir.Name.EndsWith("_Incoming") && !partitionDir.Name.Equals("temp")
					 && !partitionDir.Name.Equals("ApplicationLog") && !partitionDir.Name.Equals("AlertLog"))
	                    Platform.Log(LogLevel.Error, "Unknown partition folder '{0}' in filesystem: {1}", partitionDir.Name,
                                 filesystem.Description);
                    continue;
                }

                // Since we found a partition, we should find a rules engine too.
                ServerRulesEngine engine = _engines[partition];
            	ServerRulesEngine postArchivalEngine = _postArchivalEngines[partition];

				foreach (DirectoryInfo dateDir in partitionDir.GetDirectories())
				{
					if (dateDir.FullName.EndsWith("Deleted")
						|| dateDir.FullName.EndsWith(ServerPlatform.ReconcileStorageFolder))
						continue;

					foreach (DirectoryInfo studyDir in dateDir.GetDirectories())
					{
						String studyInstanceUid = studyDir.Name;
						try
						{
							StudyStorageLocation location = LoadStorageLocation(partition.GetKey(), studyInstanceUid);
							if (location == null)
							{
								foreach (DirectoryInfo seriesDir in studyDir.GetDirectories())
								{
									FileInfo[] sopInstanceFiles = seriesDir.GetFiles("*.dcm");

									DicomFile file = null;
									foreach (FileInfo sopFile in sopInstanceFiles)
									{
										if (!sopFile.FullName.EndsWith(ServerPlatform.DicomFileExtension))
											continue;

										try
										{
											file = new DicomFile(sopFile.FullName);
											file.Load(DicomTags.StudyId, DicomReadOptions.DoNotStorePixelDataInDataSet);
											break;
										}
										catch (Exception e)
										{
											Platform.Log(LogLevel.Warn, e, "Unexpected failure loading file: {0}.  Continuing to next file.",
											             sopFile.FullName);
											file = null;
										}
									}
									if (file != null)
									{
										studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
										break;
									}
								}

								location = LoadStorageLocation(partition.GetKey(), studyInstanceUid);
								if (location == null)
									continue;
							}

							ProcessStudy(partition, location, engine, postArchivalEngine);
							_stats.NumStudies++;

							if (CancelPending) return;
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Unexpected error while processing study: {0} on partition {1}.", studyInstanceUid,
							             partition.Description);
						}
					}

					// Cleanup the directory, if its empty.
					DirectoryUtility.DeleteIfEmpty(dateDir.FullName);
				}
            }
        }

        /// <summary>
        /// Get a ServerPartition object from the list of loaded partitions.
        /// </summary>
        /// <param name="partitionFolderName">The folder for the partition.</param>
        /// <param name="partition">The ServerPartition object.</param>
        /// <returns>true on success.</returns>
        private bool GetServerPartition(string partitionFolderName, out ServerPartition partition)
        {
            foreach (ServerPartition part in _engines.Keys)
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
        /// Load the <see cref="ServerRulesEngine"/> for each partition.
        /// </summary>
        private void LoadRulesEngine()
        {
            IServerPartitionEntityBroker broker = ReadContext.GetBroker<IServerPartitionEntityBroker>();
            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
            IList<ServerPartition> partitions = broker.Find(criteria);

            foreach (ServerPartition partition in partitions)
            {
                ServerRulesEngine engine =
                    new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyProcessed, partition.GetKey());
                _engines.Add(partition, engine);

                engine.Load();

            	engine =
            		new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyArchived, partition.GetKey());
				_postArchivalEngines.Add(partition, engine);

				engine.Load();
            }            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Process the <see cref="ServiceLock"/> item.
        /// </summary>
        /// <param name="item"></param>
        protected override void OnProcess(Model.ServiceLock item)
        {
        	LoadRulesEngine();

        	ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

        	_stats.StudyRate.Start();
        	_stats.Filesystem = info.Filesystem.Description;

        	Platform.Log(LogLevel.Info, "Starting reprocess of filesystem: {0}", info.Filesystem.Description);

        	ReprocessFilesystem(info.Filesystem);

        	Platform.Log(LogLevel.Info, "Completed reprocess of filesystem: {0}", info.Filesystem.Description);

        	_stats.StudyRate.SetData(_stats.NumStudies);
        	_stats.StudyRate.End();

        	StatisticsLogger.Log(LogLevel.Info, _stats);

        	item.ScheduledTime = item.ScheduledTime.AddDays(1);

			if (CancelPending)
			{
				Platform.Log(LogLevel.Info,
				             "Filesystem Reprocess of {0} has been canceled, rescheduling.  Entire filesystem will be reprocessed.",
				             info.Filesystem.Description);
				UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
			}
			else
	        	UnlockServiceLock(item, false, Platform.Time.AddDays(1));
        }

    	public new void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }
}
