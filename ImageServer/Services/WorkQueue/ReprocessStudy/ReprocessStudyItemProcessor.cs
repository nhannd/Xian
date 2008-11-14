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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

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

    public class ReprocessStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private StudyInfo _studyInfo;
        private StudyStorageLocation _newStorage;
        private readonly IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
        
        #endregion

        #region Overrriden Protected Methods
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.StudyStorageKey, "item.StudyStorageKey");

			if (!LoadStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing ReprocessStudy WorkQueue item, rescheduling");
				PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
				return;
			}

			try
			{
				ServerPartition partition = ServerPartition.Load(item.ServerPartitionKey);
				Platform.Log(LogLevel.Info, "Started reprocess of study {0} on partition {1}", StorageLocation.StudyInstanceUid,
							 partition.Description);
				CleanupDirectory();

				// Warning, this may end up causing problems w/ a long running transaction.
				// We're inserting all the WorkQueueUid entries, for a large study this may
				// take some time, and cause the WorkQueue table to be locked for awhile, 
				// and potentially causing deadlocks.  We may have to adjust the ScheduleReprocess()
				// routine to commit more often.
				using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					CleanupDatabase(item, ctx);
					ScheduleReprocess(ctx);
					ctx.Commit();
				}

				Platform.Log(LogLevel.Info, "Completed reprocessing of study {0} on partition {1}", StorageLocation.StudyInstanceUid,
				             partition.Description);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when reprocessing study: {0}", StorageLocation.StudyInstanceUid);
				Platform.Log(LogLevel.Error,"Study may be in invalid unprocessed state.  Study location: {0}",StorageLocation.GetStudyPath());
				throw;
			}
        }

        #endregion

        #region Private Methods

        private void CleanupDirectory()
        {
            _studyInfo = new StudyInfo(StorageLocation.StudyInstanceUid);

            FileProcessor.Process(StorageLocation.GetStudyPath(), "*.*", 
                delegate(string file)
                    {
                            
                        if (Path.GetExtension(file) == ".dcm")
                        {
                            DicomFile dcmFile = new DicomFile(file);
                            dcmFile.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);


                            string seriesUid = dcmFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
                            string SopInstanceUid = dcmFile.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
                            if (_studyInfo[seriesUid]!=null)
                            {
                                _studyInfo[seriesUid].AddInstance(SopInstanceUid);
                            }
                            else
                            {
                                SeriesInfo series = _studyInfo.AddSeries(seriesUid);
                                series.AddInstance(SopInstanceUid);                            
                            }
                        }
                        else
                        {
                            // only keep .dcm files
                            File.Delete(file);
                        }

                    }, true);
        }
        
        private void CleanupDatabase(Model.WorkQueue item, IUpdateContext context)
        {
            IDeleteStudyStorage broker = context.GetBroker<IDeleteStudyStorage>();
            DeleteStudyStorageParameters parms = new DeleteStudyStorageParameters();

            parms.ServerPartitionKey = item.ServerPartitionKey;
            parms.StudyStorageKey = item.StudyStorageKey;
            if (!broker.Execute(parms))
                throw new ApplicationException(String.Format("Cannot cleanup database for study: {0}",StorageLocation.StudyInstanceUid));

            // create new study storage
            IInsertStudyStorage insertBroker = context.GetBroker<IInsertStudyStorage>();
            InsertStudyStorageParameters insertParams = new InsertStudyStorageParameters();
            insertParams.FilesystemKey = StorageLocation.FilesystemKey;
            insertParams.Folder = StorageLocation.StudyFolder;
            insertParams.ServerPartitionKey = StorageLocation.ServerPartitionKey;
            insertParams.StudyInstanceUid = StorageLocation.StudyInstanceUid;
            insertParams.StudyStatusEnum = StorageLocation.StudyStatusEnum;
			insertParams.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
            insertParams.TransferSyntaxUid = StorageLocation.TransferSyntaxUid;

            _newStorage = insertBroker.FindOne(insertParams);

			if (_newStorage == null)
				throw new ApplicationException(String.Format("Cannot update database for study: {0}", StorageLocation.StudyInstanceUid));
        }

		private void ScheduleReprocess(IUpdateContext ctx)
		{
			ILockStudy lockStudy = ctx.GetBroker<ILockStudy>();
			LockStudyParameters lockParms = new LockStudyParameters();
			lockParms.StudyStorageKey = _newStorage.Key;
			lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ProcessingScheduled;
			if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
			{
				Platform.Log(LogLevel.Error, "Unable to lock StudyProcess entry for study {0}", _newStorage.StudyInstanceUid);
			}

			foreach (SeriesInfo series in _studyInfo.Series)
			{
				IInsertWorkQueue broker = ctx.GetBroker<IInsertWorkQueue>();

				foreach (SopInfo sop in series.Instances)
				{
					InsertWorkQueueParameters parms = new InsertWorkQueueParameters();
					parms.WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess;
					parms.Duplicate = false;
					parms.ExpirationTime =
						Platform.Time.Add(TimeSpan.FromSeconds(WorkQueueSettings.Instance.WorkQueueExpireDelaySeconds));
					parms.ScheduledTime = Platform.Time;
					parms.SeriesInstanceUid = series.SeriesUid;
					parms.ServerPartitionKey = _newStorage.ServerPartitionKey;
					parms.SopInstanceUid = sop.SopInstanceUid;
					parms.StudyStorageKey = _newStorage.Key;
					parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;
					broker.FindOne(parms);
				}
			}
		}

    	#endregion

        protected override bool CanStart()
        {
            return true;// can start anytime
        }
    }
}
