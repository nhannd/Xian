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
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
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

		private static void CheckNearline(string studyInstanceUid, ServerPartition partition)
		{
			StudyStorage storage = StudyStorage.Load(partition.Key, studyInstanceUid);
            
            if (storage==null)
            {
                throw new StudyNotFoundException(String.Format(SR.FaultNotExists, studyInstanceUid, partition.AeTitle));
            }

			if (storage.StudyStatusEnum.Equals(StudyStatusEnum.Nearline))
			{
				ServerHelper.InsertRestoreRequest(storage);
			}
		}

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
				if (!FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(partition.Key, studyInstanceUid, false, out location))
				{
					CheckNearline(studyInstanceUid, partition);
				    throw new StudyNotOnlineException();
				}
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
                        if (FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(partition.Key, studyInstanceUid, false, out location))
                        {
                            cache.Insert(location, studyInstanceUid);
                            Platform.Log(LogLevel.Info, "Cache (since {0}): Hits {1} [{3:0}%], Miss {2}",
                                         _statistics.StartTime,
                                         _statistics.Hits, _statistics.Misses,
                                         (float)_statistics.Hits / (_statistics.Hits + _statistics.Misses) * 100f);
                        }
                        else
                        {
                            CheckNearline(studyInstanceUid, partition);
                            throw new StudyNotOnlineException();
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