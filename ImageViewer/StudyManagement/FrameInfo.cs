﻿#region License

//Copyright (C)  2012 Aaron Boxer

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using ClearCanvas.Common;
using DataCache;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    public class FrameInfo : FrameHeader
    {
        public FrameInfo(int frameNumber, bool cacheable, string cacheIdTopLevel, string cacheId)
        {
            FrameNumber = frameNumber;
            Cacheable = cacheable;
            CacheIdTopLevel = cacheIdTopLevel;
            CacheId = cacheId;
            ArgBufferSize = GetContext.UnsetConversionBufferSize;
        }

        public int FrameNumber { get; private set; }
        

        #region Cache

        public bool Cacheable { get; private set; }
        public string CacheId { get; private set; }
        public string CacheIdTopLevel { get; private set; }

        public bool Cached
        {
            get
            {
                var cached = true;
                if (Cacheable)
                    cached = Frame.Cache.IsDiskCacheEnabled &&
                             Frame.Cache.IsCachedToDisk(CacheType.Pixels, CacheIdTopLevel, CacheId);
                return cached;
            }
        }

        public int ArgBufferSize { get; set; }

        public static string GenerateCacheId(string sopInstanceUid, int frameNumber)
        {
            return sopInstanceUid + "_" + frameNumber;
        }

        #endregion
    }
}