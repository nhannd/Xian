#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Web;
using System.Web.Caching;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class StudyStorageCache
    {
        private TimeSpan _retentionTime = TimeSpan.FromSeconds(10);

        private readonly Cache _cache = HttpRuntime.Cache;

        public TimeSpan RetentionTime
        {
            get { return _retentionTime; }
            set { _retentionTime = value; }
        }

        public void Insert(StudyStorageLocation storageLocation, string studyInstanceUid)
        {
            lock (_cache)
            {

                _cache.Add(studyInstanceUid, storageLocation, null, Cache.NoAbsoluteExpiration, _retentionTime, CacheItemPriority.Normal, null);
            }
        }

        public StudyStorageLocation Find(string studyInstanceUId)
        {
            lock (_cache)
            {
                object cached = _cache.Get(studyInstanceUId);
                if (cached != null)
                {
                    return cached as StudyStorageLocation;
                }
                else
                    return null;
            }
        }
    }
}