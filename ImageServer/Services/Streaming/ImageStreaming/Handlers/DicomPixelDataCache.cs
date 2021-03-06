#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    internal class DicomPixelDataCache
    {
        private static TimeSpan _retentionTime = TimeSpan.FromSeconds(10);
        private static readonly Cache _cache = HttpRuntime.Cache;

        public static TimeSpan RetentionTime
        {
            get { return _retentionTime; }
            set { _retentionTime = value; }
        }

        static public void Insert(StudyStorageLocation storageLocation, string studyInstanceUId, string seriesInstanceUid, string sopInstanceUid,  DicomPixelData pixeldata)
        {
            lock (_cache)
            {
                string key =
                    String.Format("{0}/{1}/{2}/{3}", storageLocation.GetKey().Key, studyInstanceUId, seriesInstanceUid, sopInstanceUid);

                _cache.Add(key, pixeldata, null, Cache.NoAbsoluteExpiration, _retentionTime, CacheItemPriority.Normal, null);
            }
        }

        static public DicomPixelData Find(StudyStorageLocation storageLocation, string studyInstanceUId, string seriesInstanceUid, string sopInstanceUid )
        {
            lock (_cache)
            {
                string key =
                    String.Format("{0}/{1}/{2}/{3}", storageLocation.GetKey().Key, studyInstanceUId, seriesInstanceUid, sopInstanceUid);
                
                object cachedPD = _cache.Get(key);
                if (cachedPD != null)
                {
                    //Platform.Log(LogLevel.Info, "Pixel data found in cache");
                    return cachedPD as DicomPixelData;
                }
                else
                    return null;
            }
        }
    }
}