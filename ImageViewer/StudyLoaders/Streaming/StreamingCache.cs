

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Caching;
using DataCache;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
    internal class CacheLogger : ICacheLogger
    {
        public void Log(CacheLogLevel level, string message)
        {
            var logLevel = LogLevel.Debug;
            switch (level)
            {
                case CacheLogLevel.Error:
                    logLevel = LogLevel.Error;
                    break;
                case CacheLogLevel.Info:
                    logLevel = LogLevel.Info;
                    break;
                case CacheLogLevel.Debug:
                    logLevel = LogLevel.Debug;
                    break;
                case CacheLogLevel.Warn:
                    logLevel = LogLevel.Warn;
                    break;
            }
            Platform.Log(logLevel, message);
        }
    }


    [ExtensionOf(typeof (StreamingCacheExtensionPoint))]
    public class StreamingCache : IStreamingCache
    {
        private readonly IUnifiedCache _cacheImpl;
        public StreamingCache()
        {
            _cacheImpl = new UnifiedCache(new CacheLogger());
        }
        public bool IsDiskCacheEnabled
        {
            get { return _cacheImpl.IsDiskCacheEnabled; }
        }

        public StreamByteBufferCacheItem Get(StreamCacheType cacheType, string topLevelId, string cacheId, StreamGetContext context)
        {
            return Convert(_cacheImpl.Get(Convert(cacheType), topLevelId, cacheId, Convert(context)));
        }

        public StreamPutResponse Put(string topLevelId, string cacheId, StreamByteBufferCacheItem byteBufferCacheItem)
        {
            return Convert(_cacheImpl.Put(topLevelId, cacheId, Convert(byteBufferCacheItem)));
        }

        public void Put(string cacheId, StreamByteBufferCacheItem byteBufferCacheItem)
        {
            _cacheImpl.Put(cacheId, Convert(byteBufferCacheItem));
        }

        public StreamByteBufferCacheItem Get(string cacheId)
        {
            return Convert(_cacheImpl.Get(cacheId));
        }

        public StreamPutResponse Put(string topLevelId, string cacheId, StreamStringCacheItem stringCacheItem)
        {
            return Convert(_cacheImpl.Put(topLevelId, cacheId, Convert(stringCacheItem)));
        }

        public bool IsCachedToDisk(StreamCacheType type, string topLevelId, string cacheId)
        {
            return _cacheImpl.IsCachedToDisk(Convert(type), topLevelId, cacheId);
        }

        public void ClearFromMemory(string cacheId)
        {
            _cacheImpl.ClearFromMemory(cacheId);
        }

        public IEnumerable<string> EnumerateCachedItems(string topLevelId)
        {
            return _cacheImpl.EnumerateCachedItems(topLevelId);
        }

        #region Converters

        GetContext Convert(StreamGetContext context)
        {
            return context == null
                       ? null
                       : new GetContext
                             {
                                 Decompressor = context.Decompressor,
                                 PostProcessor = context.PostProcessor,
                                 ConversionBufferSize = context.ConversionBufferSize
                             };
        }

        StreamPutResponse Convert(PutResponse response)
        {
           switch(response)
           {
               case PutResponse.Error:
                   return StreamPutResponse.Error;
               case PutResponse.Disabled:
                   return StreamPutResponse.Disabled;
               case PutResponse.Success:
                   return StreamPutResponse.Success;
                case PutResponse.InvalidData:
                   return StreamPutResponse.InvalidData;
               default:
                   return StreamPutResponse.Error;
           }
        }

        CacheType Convert(StreamCacheType cacheType)
        {
            switch (cacheType)
            {
                case StreamCacheType.String:
                    return CacheType.String;
                case StreamCacheType.Pixels:
                    return CacheType.Pixels;
                default:
                    return CacheType.Pixels;
            }
        }

        ByteBufferCacheItem Convert(StreamByteBufferCacheItem item)
        {
            return item == null ? 
                        null : 
                        new ByteBufferCacheItem
                       {
                           IsCompressed = item.IsCompressed,
                           Data = item.Data,
                           Size = item.Size,
                           ByteStream =  item.ByteStream
                       };
        }
        StreamByteBufferCacheItem Convert(ByteBufferCacheItem item)
        {
            return item == null ?
                     null : 
                       new StreamByteBufferCacheItem
                        {
                            IsCompressed = item.IsCompressed,
                            Data = item.Data,
                            Size = item.Size,
                            ByteStream = item.ByteStream
                        };
        }
        StringCacheItem Convert(StreamStringCacheItem item)
        {
            return item == null ?
                     null :
                       new StringCacheItem
                       {
                           Data = item.Data
                       };
        }

        #endregion
    }
}