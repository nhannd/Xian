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
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReprocessStudy
{
    /// <summary>
    /// Information about the study being reprocessed
    /// </summary>
    class StudyInfo
    {
        #region Private Members
        private string _studyInstanceUid;
        private readonly Dictionary<string, SeriesInfo> _series = new Dictionary<string, SeriesInfo>();
        #endregion

        #region Constructors
        
        public StudyInfo(string studyInstanceUid)
        {
            _studyInstanceUid = studyInstanceUid;
        }

        #endregion

        #region Public Properties

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public SeriesInfo this[string seriesUid]
        {
            get
            {
                if (!_series.ContainsKey(seriesUid))
                {
                    return null;
                }

                return _series[seriesUid];
            }
            set
            {
                if (_series.ContainsKey(seriesUid))
                    _series[seriesUid] = value;
                else
                    _series.Add(seriesUid, value);
            }
        }

        public IEnumerable<SeriesInfo> Series
        {
            get { return _series.Values; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a series with specified series instance uid into the study
        /// </summary>
        /// <param name="seriesUid"></param>
        /// <returns></returns>
        public SeriesInfo AddSeries(string seriesUid)
        {
            SeriesInfo series = new SeriesInfo(seriesUid);
            _series.Add(seriesUid, series);

            return series;
        }

        #endregion
    }

    /// <summary>
    /// Information about the series of the study being reprocessed
    /// </summary>
    class SeriesInfo
	{
		#region Private Members
		private string _seriesUid;
        private readonly Dictionary<string, SopInfo> _sopInstances = new Dictionary<string, SopInfo>();
		#endregion

		#region Constructors
		public SeriesInfo(string seriesUid)
        {
            _seriesUid = seriesUid;
		}
		#endregion

		#region Public Properties
		public string SeriesUid
        {
            get { return _seriesUid; }
            set { _seriesUid = value; }
        }

        public SopInfo this[string sopUid]
        {
            get
            {
                if (_sopInstances.ContainsKey(sopUid))
                    return _sopInstances[sopUid];
                else
                    return null;
            }
            set
            {
                if (_sopInstances.ContainsKey(sopUid))
                    _sopInstances[sopUid] = value;
                else
                    _sopInstances.Add(sopUid, value);
            }
        }

        public IEnumerable<SopInfo> Instances
        {
            get { return _sopInstances.Values; }
		}
		#endregion

		#region Public Methods
		public SopInfo AddInstance(string instanceUid)
        {
            SopInfo sop = new SopInfo(instanceUid);
            _sopInstances.Add(instanceUid, sop);

            return sop;
		}
		#endregion
	}

    /// <summary>
    /// Information about the sop instance being reprocessed.
    /// </summary>
    class SopInfo
    {
        #region Private Members
        private string _sopInstanceUid;
        #endregion

        #region Constructors
        public SopInfo(string instanceUid)
        {
            _sopInstanceUid = instanceUid;
        }
        #endregion

        #region Public Properties

        public string SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }

        #endregion
    }



    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class ReprocessStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members

        private ReprocessStudyQueueData _queueData; 
        
        #endregion

        #region Overrriden Protected Methods
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.StudyStorageKey, "item.StudyStorageKey");

            StudyProcessorContext context = new StudyProcessorContext(StorageLocation);
            SopInstanceProcessor processor = new SopInstanceProcessor(context);
            Dictionary<string, List<string>> seriesMap = new Dictionary<string, List<string>>();

            bool successful = true;
            string failureDescription = null;
            bool completed = false;

            // The processor stores its state in the Data column
            LoadState(item);
            
            StudyXml studyXml = LoadStudyXml();

            if (_queueData.State == null || !_queueData.State.ExecuteAtLeastOnce)
            {
				if (Study == null)
					Platform.Log(LogLevel.Info,
					             "Reprocessing study {0} on Partition {1}", StorageLocation.StudyInstanceUid,
					             ServerPartition.Description);
				else
					Platform.Log(LogLevel.Info,
					             "Reprocessing study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
					             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
					             Study.AccessionNumber, ServerPartition.Description);

                CleanupDatabase();
            }
            else
            {
                if (_queueData.State.Completed)
                {
                    #region SAFE-GUARD CODE: PREVENT INFINITE LOOP

                    // The processor indicated it had completed reprocessing in previous run. The entry should have been removed and this block of code should never be called.
                    // However, we have seen ReprocessStudy entries that mysterously contain rows in the WorkQueueUid table.
                    // The rows prevent the entry from being removed from the database and the ReprocessStudy keeps repeating itself.

                    
                    // update the state first, increment the CompleteAttemptCount
                    _queueData.State.ExecuteAtLeastOnce = true;
                    _queueData.State.Completed = true;
                    _queueData.State.CompleteAttemptCount++;
                    SaveState(item, _queueData);

                    if (_queueData.State.CompleteAttemptCount < 10)
                    {
                        // maybe there was db error in previous attempt to remove the entry. Let's try again.
                        Platform.Log(LogLevel.Info, "Resuming Reprocessing study {0} but it was already completed!!!", StorageLocation.StudyInstanceUid);
                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    }
                    else
                    {
                        // we are definitely stuck.
                        Platform.Log(LogLevel.Error, "ReprocessStudy {0} for study {1} appears stuck. Aborting it.", item.Key, StorageLocation.StudyInstanceUid);
                        item.FailureDescription = "This entry had completed but could not be removed.";
                        PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                    }

                    return;

                    #endregion
                }

                if (Study == null)
					Platform.Log(LogLevel.Info,
								 "Resuming Reprocessing study {0} on Partition {1}", StorageLocation.StudyInstanceUid,
								 ServerPartition.Description);
				else
					Platform.Log(LogLevel.Info,
                             "Resuming Reprocessing study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);
                
            }

            int reprocessedCounter = 0;
            
            try
            {
                // Traverse the directories, process 500 files at a time
                FileProcessor.Process(StorageLocation.GetStudyPath(), "*.*",
                                      delegate(string path, out bool cancel)
                                          {
                                              #region Reprocess File

                                              FileInfo file = new FileInfo(path);
                                              
                                              // ignore all files except those ending ".dcm"
                                              // ignore "bad(0).dcm" files too
                                              if (Regex.IsMatch(file.Name.ToUpper(), "[0-9]+\\.DCM$"))
                                              {
                                                  try
                                                  {
                                                      DicomFile dicomFile = new DicomFile(path);
                                                      dicomFile.Load(DicomReadOptions.StorePixelDataReferences);
                                                      
                                                      string seriesUid = dicomFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
                                                      string instanceUid =dicomFile.DataSet[DicomTags.SopInstanceUid].GetString(0,string.Empty);
                                                      if (studyXml.Contains(seriesUid, instanceUid))
                                                      {
                                                          if (!seriesMap.ContainsKey(seriesUid))
                                                          {
                                                              seriesMap.Add(seriesUid, new List<string>());
                                                          }
                                                          if (!seriesMap[seriesUid].Contains(instanceUid))
                                                              seriesMap[seriesUid].Add(instanceUid);
                                                          else
                                                          {
                                                              Platform.Log(LogLevel.Warn, "SOP Instance UID in {0} appears more than once in the study.", path);
                                                          }
                                                      }
                                                      else
                                                      {
                                                          Platform.Log(ServerPlatform.InstanceLogLevel, "Reprocessing SOP {0} for study {1}",instanceUid, StorageLocation.StudyInstanceUid);
                                                          string groupID = ServerHelper.GetUidGroup(dicomFile, StorageLocation.ServerPartition, WorkQueueItem.InsertTime);
                                                          ProcessingResult result = processor.ProcessFile(groupID, dicomFile, studyXml, false, true, null, null);
                                                          switch (result.Status)
                                                          {
                                                              case ProcessingStatus.Success:
                                                                  reprocessedCounter++;
                                                                  if (!seriesMap.ContainsKey(seriesUid))
                                                                  {
                                                                      seriesMap.Add(seriesUid, new List<string>());
                                                                  }

                                                                  if (!seriesMap[seriesUid].Contains(instanceUid))
                                                                      seriesMap[seriesUid].Add(instanceUid);
                                                                  else
                                                                  {
                                                                      Platform.Log(LogLevel.Warn, "SOP Instance UID in {0} appears more than once in the study.", path);
                                                                  }
                                                                  break;

                                                              case ProcessingStatus.Failed:
                                                                  Platform.Log(LogLevel.Error, "Failed to reprocess SOP {0} for study {1}", instanceUid, StorageLocation.StudyInstanceUid);
                                                                  // failureDescription = ""; TODO: augment the processor to return the error
                                                                  failureDescription = String.Format("Failed to reprocess SOP {0}", instanceUid);


                                                                  break;
                                                          }
                                                      }
                                                  }
                                                  catch (DicomException ex)
                                                  {
                                                      Platform.Log(LogLevel.Warn, "Skip reprocessing and delete {0}: Not readable.", path);
                                                      FileUtils.Delete(path);
                                                      failureDescription = ex.Message;

                                                      RaiseAlert(item, AlertLevel.Critical,
                                                                 "File {0} is not readable and has been removed from study.",
                                                                 path, StorageLocation.Study.AccessionNumber,
                                                                 StorageLocation.Study.PatientsName,
                                                                 StorageLocation.Study.PatientId);
                                                  }
                                              }
                                              else if (!file.Extension.Equals(".xml") && !file.Extension.Equals(".gz"))
                                              {
                                                  // not a ".dcm" or header file, delete it
                                                  FileUtils.Delete(path);
                                              }

                                              #endregion

                                              cancel = reprocessedCounter >= 500;
                                          }, true);

                EnsureConsistentObjectCount(studyXml, seriesMap);
                SaveStudyXml(studyXml);

                // Completed if either all files have been reprocessed 
                // or no more dicom files left that can be reprocessed.
                completed = reprocessedCounter == 0;
                successful = true;
            }
            catch (Exception e)
            {
                successful = false;
                failureDescription = e.Message;
                Platform.Log(LogLevel.Error, e, "Unexpected exception when reprocessing study: {0}", StorageLocation.StudyInstanceUid);
                Platform.Log(LogLevel.Error, "Study may be in invalid unprocessed state.  Study location: {0}", StorageLocation.GetStudyPath());
                throw;
            }
            finally
            {
                // Update the state
                _queueData.State.ExecuteAtLeastOnce = true;
                _queueData.State.Completed = completed;
                _queueData.State.CompleteAttemptCount++;
                SaveState(item, _queueData);
                    
                if (!successful)
                {
                    FailQueueItem(item, failureDescription);
                }
                else 
                {
                    if (!completed)
                    {
                        // Put it back to Pending
                        PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                    }
                    else
                    {
                        LogHistory();

                        // Run Study / Series Rules Engine.
                        StudyRulesEngine engine = new StudyRulesEngine(StorageLocation, ServerPartition);
                        engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);

                        // Log the FilesystemQueue related entries
                        StorageLocation.LogFilesystemQueue();

                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);

                        Platform.Log(LogLevel.Info, "Completed reprocessing of study {0} on partition {1}", StorageLocation.StudyInstanceUid, ServerPartition.Description);
                        
                    }
                
                }
            }
        }

        private void SaveState(Model.WorkQueue item, ReprocessStudyQueueData queueData)
        {
            // Update the queue state
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                queueData.State.ExecuteAtLeastOnce = true;
                IWorkQueueEntityBroker broker = updateContext.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueUpdateColumns parms = new WorkQueueUpdateColumns();
                parms.Data = XmlUtils.SerializeAsXmlDoc(_queueData);
                broker.Update(item.GetKey(), parms);
                updateContext.Commit();
            }
        }

        private void CleanupDatabase()
        {
            // Delete StudyStorage related tables except the StudyStorage table itself
            // This will reset the object count in all levels. The Study, Patient, Series
            // records will be recreated when the file is reprocessed.
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IResetStudyStorage broker = updateContext.GetBroker<IResetStudyStorage>();
                ResetStudyStorageParameters criteria = new ResetStudyStorageParameters();
                criteria.StudyStorageKey = StorageLocation.GetKey();
                if (!broker.Execute(criteria))
                {
                    throw new ApplicationException("Could not reset study storage");
                }
                else
                    updateContext.Commit();
            }
        }

        private void LogHistory()
        {
            Platform.Log(LogLevel.Debug, "Logging history record...");
            using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // create the change log based on info stored in the queue entry
                ReprocessStudyChangeLog changeLog = new ReprocessStudyChangeLog();
                changeLog.TimeStamp = Platform.Time;
                changeLog.StudyInstanceUid = StorageLocation.StudyInstanceUid;
                changeLog.Reason = _queueData.ChangeLog != null ? _queueData.ChangeLog.Reason : "N/A";
                changeLog.User = _queueData.ChangeLog != null ? _queueData.ChangeLog.User : "Unknown";
                
                StudyHistory history = ServerPlatform.CreateStudyHistoryRecord(ctx, StorageLocation, null, StudyHistoryTypeEnum.Reprocessed, null, changeLog);
                if (history != null)
                    ctx.Commit();
            }
        }

        private void SaveStudyXml(StudyXml studyXml)
        {
            XmlDocument doc = studyXml.GetMemento(new StudyXmlOutputSettings());
            using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(StorageLocation.GetStudyXmlPath(), FileMode.Create),
                                      gzipStream = FileStreamOpener.OpenForSoleUpdate(StorageLocation.GetCompressedStudyXmlPath(), FileMode.Create))
            {
                StudyXmlIo.WriteXmlAndGzip(doc, xmlStream, gzipStream);
                xmlStream.Close();
                gzipStream.Close();
            }
        }

        private StudyXml LoadStudyXml()
        {
            StudyXml studyXml;
            if (!_queueData.State.ExecuteAtLeastOnce)
            {
                FileUtils.Delete(StorageLocation.GetCompressedStudyXmlPath());
                FileUtils.Delete(StorageLocation.GetStudyXmlPath());
                studyXml = new StudyXml(StorageLocation.StudyInstanceUid);

            }
            else
            {
                // reuse the current xml and add more sop into it as we find
                studyXml = StorageLocation.LoadStudyXml();
                   
            }

            // If for some reason, the xml does not exist, recreate it
            if (studyXml == null)
            {
                studyXml = new StudyXml(StorageLocation.StudyInstanceUid);
            }
            return studyXml;
        }

        private void LoadState(Model.WorkQueue item)
        {
            if (item.Data!=null)
            {
                _queueData = XmlUtils.Deserialize<ReprocessStudyQueueData>(item.Data);
            }

            if (_queueData == null)
            {
                _queueData = new ReprocessStudyQueueData();
                _queueData.State = new ReprocessStudyState();
                _queueData.State.ExecuteAtLeastOnce = false;
            }
        }

        private void EnsureConsistentObjectCount(StudyXml studyXml, IDictionary<string, List<string>> processedSeriesMap)
        {
            // We have to ensure that the counts in studyXml and what we have processed are consistent.
            // Files or folder may be reprocessed but then become missing when then entry is resumed.
            // We have to removed them from the studyXml before committing the it.

            Platform.Log(LogLevel.Info, "Verifying study xml against the filesystems");
            int filesProcessed = 0;
            foreach (string seriesUid in processedSeriesMap.Keys)
            {
                filesProcessed += processedSeriesMap[seriesUid].Count;
            }

            // Used to keep track of the series to be removed.
            // We can't remove the item from the study xml while we are 
            // interating through it
            List<string> seriesToRemove = new List<string>();
            foreach(SeriesXml seriesXml in studyXml)
            {
                if (!processedSeriesMap.ContainsKey(seriesXml.SeriesInstanceUid))
                {
                    seriesToRemove.Add(seriesXml.SeriesInstanceUid);
                }
                else
                {
                    //check all instance in the series
                    List<string> foundInstances = processedSeriesMap[seriesXml.SeriesInstanceUid];
                    List<string> instanceToRemove = new List<string>();
                    foreach (InstanceXml instanceXml in seriesXml)
                    {
                        if (!foundInstances.Contains(instanceXml.SopInstanceUid))
                        {
                            // the sop no long exists in the filesystem
                            instanceToRemove.Add(instanceXml.SopInstanceUid);
                        }
                    }

                    foreach(string instanceUid in instanceToRemove)
                    {
                        seriesXml[instanceUid] = null;
                        Platform.Log(LogLevel.Info, "Removed SOP {0} in the study xml: it no longer exists.", instanceUid);
                    }
                }
            }

            foreach(string seriesUid in seriesToRemove)
            {
                studyXml[seriesUid] = null;
                Platform.Log(LogLevel.Info, "Removed Series {0} in the study xml: it no longer exists.", seriesUid);
                    
            }

            Platform.CheckTrue(studyXml.NumberOfStudyRelatedSeries == processedSeriesMap.Count,
               String.Format("Number of series in the xml do not match number of series reprocessed: {0} vs {1}",
               studyXml.NumberOfStudyRelatedInstances, processedSeriesMap.Count));
            
            Platform.CheckTrue(studyXml.NumberOfStudyRelatedInstances == filesProcessed, 
                String.Format("Number of instances in the xml do not match number of reprocessed: {0} vs {1}",
                studyXml.NumberOfStudyRelatedInstances, filesProcessed));
           
            // update the instance count in the db
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IStudyEntityBroker broker = updateContext.GetBroker<IStudyEntityBroker>();
                StudyUpdateColumns columns = new StudyUpdateColumns();
                columns.NumberOfStudyRelatedInstances = studyXml.NumberOfStudyRelatedInstances;
                columns.NumberOfStudyRelatedSeries = studyXml.NumberOfStudyRelatedSeries;
                broker.Update(StorageLocation.Study.GetKey(), columns);
                updateContext.Commit();
            }
        }

        #endregion

        protected override bool CanStart()
        {
            return true;// can start anytime
        }
    }
}
