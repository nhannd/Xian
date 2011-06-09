#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Caching;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailCache
    {
        void Put(string key, IThumbnailData thumbnail);
        IThumbnailData Get(string key);
        void Remove(string key);
        void Clear();
    }

    public class ThumbnailCache : IThumbnailCache
    {
        private const string _defaultRegionId = "default";

        private readonly string _cacheId;
        private readonly string _regionId;
        
        private ThumbnailCache(string cacheId, string regionId)
        {
            _cacheId = cacheId;
            _regionId = regionId;
            
            Expiration = TimeSpan.FromMinutes(5);
            UseSlidingExpiration = true;
        }

        public TimeSpan Expiration { get; set; }
        public bool UseSlidingExpiration { get; set; }

        public static bool IsSupported
        {
            get { return Cache.IsSupported(); }
        }

        public static IThumbnailCache Create(string cacheId)
        {
            return Create(cacheId, _defaultRegionId);
        }

        public static IThumbnailCache Create(string cacheId, string regionId)
        {
            if (!IsSupported)
                throw new NotSupportedException("Caching is not currently supported.");

            return new ThumbnailCache(cacheId, regionId);
        }

        public void Put(string key, IThumbnailData thumbnail)
        {
            WithCacheClient(client => client.Put(key, thumbnail, new CachePutOptions(_regionId, Expiration, UseSlidingExpiration)));
        }

        public IThumbnailData Get(string key)
        {
            IThumbnailData thumb = null;
            WithCacheClient(client => thumb = client.Get(key, new CacheGetOptions(_regionId)) as IThumbnailData);
            return thumb;
        }

        public void Remove(string key)
        {
            WithCacheClient(client => client.Remove(key, new CacheRemoveOptions(_regionId)));
        }

        public void Clear()
        {
            WithCacheClient(client => client.ClearCache());
        }
        
        private void WithCacheClient(Action<ICacheClient> withCacheClient)
        {
            using (var client = Cache.CreateClient(_cacheId))
            {
                withCacheClient(client);
            }
        }
    }
}