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
using System.Text;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    internal class DicomPixelDataCache
    {
        private static TimeSpan _retentionTime = TimeSpan.FromSeconds(10);

        private static Cache _cache = HttpRuntime.Cache;

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