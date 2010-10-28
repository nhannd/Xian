#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class ServerTransientError:Exception
    {
        public ServerTransientError(){}

        public ServerTransientError(String message)
            : base(message)
        {
        }
        public ServerTransientError(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class StudyNotFoundException : Exception
    {
        public StudyNotFoundException(String message) : base(message)
        {
        }
    }

    public class StudyNotOnlineException : ServerTransientError
    {
        public StudyNotOnlineException(String message)
            : base(message)
        {
        }

        public StudyNotOnlineException()
        {
        }
    }

    public class StudyAccessException : ServerTransientError
    {
        private QueueStudyStateEnum _studyState; 
        public StudyAccessException(String message, QueueStudyStateEnum state, Exception innerException)
            : base(message, innerException)
        {
            _studyState = state;
        }

        public QueueStudyStateEnum StudyState
        {
            get { return _studyState; }
            set { _studyState = value; }
        }
    }

    class StudyStorageLoader
	{
		#region Private Members
		private static readonly ServerSessionList _serverSessions = new ServerSessionList();
        private static readonly StudyStorageLoaderStatistics _statistics = new StudyStorageLoaderStatistics();
        private readonly string _sessionId;
        private bool _cacheEnabled = true;
        private TimeSpan _cacheRetentionTime = TimeSpan.FromSeconds(10); //default
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

		#endregion

		#region Public Methods

        /// <summary>
        /// Finds the <see cref="StudyStorageLocation"/> for the specified study
        /// </summary>
        /// <param name="studyInstanceUid"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        /// 
    	public StudyStorageLocation Find(string studyInstanceUid, ServerPartition partition)
        {
            TimeSpan STATS_WINDOW = TimeSpan.FromMinutes(1);
            StudyStorageLocation location;
            if (!CacheEnabled)
            {
            	FilesystemMonitor.Instance.GetReadableStudyStorageLocation(partition.Key, studyInstanceUid,
            	                                                           StudyRestore.True, StudyCache.False,
            	                                                           out location);
            }
            else
            {
                Session session = _serverSessions[_sessionId];
                StudyStorageCache cache;
                lock (session)
                {
                    cache = session["StorageLocationCache"] as StudyStorageCache;

                    if (cache == null)
                    {
                        cache = new StudyStorageCache {RetentionTime = CacheRetentionTime};
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
                        
						FilesystemMonitor.Instance.GetReadableStudyStorageLocation(partition.Key, studyInstanceUid, StudyRestore.True,StudyCache.False, out location);

						cache.Insert(location, studyInstanceUid);
                        Platform.Log(LogLevel.Info, "Cache (since {0}): Hits {1} [{3:0}%], Miss {2}",
                                         _statistics.StartTime,
                                         _statistics.Hits, _statistics.Misses,
                                         (float)_statistics.Hits / (_statistics.Hits + _statistics.Misses) * 100f);
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
            
            }

            if (location == null)
                throw new StudyNotOnlineException(String.Format(SR.FaultStudyIsNearline, studyInstanceUid));

               
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