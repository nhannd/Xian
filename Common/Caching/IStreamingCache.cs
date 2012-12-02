#region License

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

using System;
using System.Collections.Generic;
using System.IO;


namespace ClearCanvas.Common.Caching
{

    public class StreamGetContext
    {
        public const int UnsetConversionBufferSize = -1;
        public Func<byte[], int, byte[]> Decompressor { get; set; }
        public Func<byte[], byte[]> PostProcessor { get; set; }
        public int ConversionBufferSize { get; set; }
    }

    public enum StreamCacheType
    {
        Pixels,
        String
    }

    public interface IStreamMemoryCacheItem
    {
        int Size { get; }
    }

    public class StreamStringCacheItem : IStreamMemoryCacheItem
    {
        public string Data { get; set; }

        public int Size
        {
            get { return Data.Length; }
        }
    }

    public class StreamByteBufferCacheItem : IStreamMemoryCacheItem
    {
        public bool IsCompressed;
        public byte[] Data;
        public int Size { get; set; }
        public Stream ByteStream { get; set; }
    }

    public enum StreamPutResponse
    {
        Success,
        Disabled,
        InvalidData,
        Error
    }


    public interface IStreamingCache
    {
        bool IsDiskCacheEnabled { get; }
        StreamByteBufferCacheItem Get(StreamCacheType cacheType, string topLevelId, string cacheId, StreamGetContext context);
        StreamPutResponse Put(string topLevelId, string cacheId, StreamByteBufferCacheItem byteBufferCacheItem);
        void Put(string cacheId, StreamByteBufferCacheItem byteBufferCacheItem);
        StreamByteBufferCacheItem Get(string cacheId);
        StreamPutResponse Put(string topLevelId, string cacheId, StreamStringCacheItem stringCacheItem);
        bool IsCachedToDisk(StreamCacheType type, string topLevelId, string cacheId);
        void ClearFromMemory(string cacheId);
        IEnumerable<string> EnumerateCachedItems(string topLevelId);
    }

    [ExtensionPoint()]
    public class StreamingCacheExtensionPoint : ExtensionPoint<IStreamingCache>
    {
    }

}
