using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class StudyStorageLoader
	{
		#region Private Members
		private static readonly ServerSessionList _serverSessions = new ServerSessionList();
        private static readonly StudyStorageLoaderStatistics _statistics = new StudyStorageLoaderStatistics();
        private readonly string _sessionId;
        private bool _cacheEnabled = true;
        private TimeSpan _cacheRetentionTime = TimeSpan.FromSeconds(10); //default
    	private string _faultDescription = String.Empty;
		#endregion

		#region Public Properties
		public StudyStorageLoader(string sessionId)
        {
            _sessionId = sessionId;
        }

        public bool CacheEnabled
        {
            get { return _cacheEnabled; }
            set { _cacheEnabled = value; }
        }

        public TimeSpan CacheRetentionTime
        {
            get { return _cacheRetentionTime; }
            set { _cacheRetentionTime = value; }
		}

		public string FaultDescription
		{
			get { return _faultDescription; }
			set { _faultDescription = value; }
		}
		#endregion

		#region Public Methods
		private void CheckNearline(string studyInstanceUid, ServerPartition partition)
		{
			StudyStorage storage = StudyStorage.Load(partition.Key, studyInstanceUid);
			if (storage != null && storage.StudyStatusEnum.Equals(StudyStatusEnum.Nearline))
			{
				FaultDescription = SR.FaultNearline;

				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IInsertRestoreQueue broker = updateContext.GetBroker<IInsertRestoreQueue>();

					InsertRestoreQueueParameters parms = new InsertRestoreQueueParameters();
					parms.StudyStorageKey = storage.Key;

					RestoreQueue queue = broker.FindOne(parms);

					if (queue != null)
						updateContext.Commit();
				}
			}
			else if (storage != null)
			{
				FaultDescription = SR.FaultFilesystemOffline;
			}
			else
				FaultDescription = String.Format(SR.FaultNotExists, studyInstanceUid, partition.AeTitle);
		}

    	public StudyStorageLocation Find(string studyInstanceUid, ServerPartition partition)
        {
            TimeSpan STATS_WINDOW = TimeSpan.FromMinutes(1);
            StudyStorageLocation location;
            if (!CacheEnabled)
            {
				if (!FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(partition.Key, studyInstanceUid, out location))
				{
					CheckNearline(studyInstanceUid, partition);
				}
            	return location;
            }

            Session session = _serverSessions[_sessionId];
            StudyStorageCache cache ;
            lock (session)
            {
                cache = session["StorageLocationCache"] as StudyStorageCache;

                if (cache == null)
                {
                    cache = new StudyStorageCache();
                    cache.RetentionTime = CacheRetentionTime;
                    session.Add("StorageLocationCache", cache);
                }
            }
            
            // lock the cache instead of the list so that we won't block requests from other
            // clients if we need to fetch from the database.
            lock (cache)
            {
                location = cache.Find(studyInstanceUid);
                if (location == null)
                {
                    _statistics.Misses++;
					if (FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(partition.Key, studyInstanceUid, out location))
					{
						cache.Insert(location, studyInstanceUid);
						Platform.Log(LogLevel.Info, "Cache (since {0}): Hits {1} [{3:0}%], Miss {2}",
						             _statistics.StartTime,
						             _statistics.Hits, _statistics.Misses,
						             (float) _statistics.Hits/(_statistics.Hits + _statistics.Misses)*100f);
					}
					else
					{
						CheckNearline(studyInstanceUid, partition);
					}
                }
                else
                {
                    _statistics.Hits++;
                }

                if (_statistics.ElapsedTime > STATS_WINDOW)
                {
                    _statistics.Reset();
                }
                
            }
            return location;
		}
		#endregion
	}

    class StudyStorageLoaderStatistics : StatisticsSet
    {
        public DateTime StartTime = DateTime.Now;

        public TimeSpan ElapsedTime
        {
            get
            {
                return DateTime.Now - StartTime;
            }
        }

        /// <summary>
        /// Total number of hits
        /// </summary>
        public ulong Hits
        {
            get
            {
                if (this["Hits"] == null)
                    this["Hits"] = new Statistics<ulong>("Hits");

                return (this["Hits"] as Statistics<ulong>).Value;
            }
            set
            {
                this["Hits"] = new Statistics<ulong>("Hits", value);
            }
        }

        /// <summary>
        /// Total number of misses
        /// </summary>
        public ulong Misses
        {
            get
            {
                if (this["Misses"] == null)
                    this["Misses"] = new Statistics<ulong>("Misses");

                return (this["Misses"] as Statistics<ulong>).Value;
            }
            set
            {
                this["Misses"] = new Statistics<ulong>("Misses", value);
            }
        }

        public void Reset()
        {
            StartTime = DateTime.Now;
            Hits = 0;
            Misses = 0;
        }
    }

}