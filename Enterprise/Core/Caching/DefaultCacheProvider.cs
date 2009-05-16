using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Enterprise.Common.Caching;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core.Caching
{
	[ExtensionOf(typeof(CacheProviderExtensionPoint))]
	public class DefaultCacheProvider : ICacheProvider
	{
		private System.Web.Caching.Cache _cache;

		#region ICacheProvider Members

		public void Initialize(CacheProviderInitializationArgs args)
		{
            // This may seem odd, but using the ASP.NET cache outside of an ASP app
            // is perfectly ok, according to this MSDN article:
            // http://msdn.microsoft.com/en-us/library/ms978500.aspx
			_cache = HttpRuntime.Cache;
		}

		public ICacheClient CreateClient(CacheClientCreationArgs args)
		{
            // a cacheID is required!
            Platform.CheckForEmptyString(args.CacheID, "CacheID");

            // ensure both cache and region exist
            CreateCache(args.CacheID);
            CreateRegion(args.CacheID, args.Region);

			return new DefaultCacheClient(this, args);
		}

		#endregion

        #region Internal API

        internal object Get(string cacheID, string region, string key)
		{
			Platform.CheckForNullReference(key, "key");

            string cacheKey = GetItemCacheKey(cacheID, region, key);

			object obj = _cache.Get(cacheKey);
			if (obj == null)
				return null;

			DictionaryEntry entry = (DictionaryEntry)obj;
			return key.Equals(entry.Key) ? entry.Value : null;
		}

        internal void Put(string cacheID, string region, string key, object value, TimeSpan expiration, bool sliding)
		{
			Platform.CheckForNullReference(key, "key");
			Platform.CheckForNullReference(value, "value");

            string cacheKey = GetItemCacheKey(cacheID, region, key);
            PutItem(cacheKey, key, GetRegionCacheKey(cacheID, region), value, expiration, sliding);
		}


        internal void Remove(string cacheID, string region, string key)
		{
			Platform.CheckForNullReference(key, "key");
            string cacheKey = GetItemCacheKey(cacheID, region, key);
            _cache.Remove(cacheKey);
		}

		internal void ClearRegion(string cacheID, string region)
		{
            string regionKey = GetRegionCacheKey(cacheID, region);

            // remove region key to clear all items
            _cache.Remove(regionKey);

            // re-create region
            CreateRegion(cacheID, regionKey);
		}

        internal void ClearCache(string cacheID)
        {
            string rootKey = GetRootCacheKey(cacheID);

            // remove root key to clear all items
            _cache.Remove(rootKey);

            // re-create cache
            CreateCache(cacheID);
        }

        #endregion

        #region Helpers

        private void CreateCache(string cacheID)
		{
            CreateRoot(GetRootCacheKey(cacheID), null);
		}

        private void CreateRegion(string cacheID, string region)
        {
            CreateRoot(GetRegionCacheKey(cacheID, region), GetRootCacheKey(cacheID));
        }

        private void CreateRoot(string rootKey, string dependencyKey)
        {
            // if not already stored, store it now
            if (_cache.Get(rootKey) == null)
            {
                // add region key, dependent on root key
                _cache.Add(
                    rootKey,
                    rootKey,
                    dependencyKey == null ? null : new CacheDependency(null, new string[] { dependencyKey }),
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable,
                    OnCacheItemRemoved);
            }
        }

        private static string GetRootCacheKey(string cacheid)
        {
            return cacheid;
        }

        private static string GetRegionCacheKey(string cacheid, string region)
        {
            return string.Format("{0}:{1}", cacheid, region);
        }

        private static string GetItemCacheKey(string cacheid, string region, string key)
        {
            return string.Format("{0}:{1}:{2}", cacheid, region, key);
        }

        private void PutItem(
            string qualifiedKey,
            string key,
            string dependencyKey,
            object value,
            TimeSpan expiryTime,
            bool sliding)
        {
            CacheDependency dependency = dependencyKey == null ? null
                : new CacheDependency(null, new string[] { dependencyKey });

            DateTime absExpiration = sliding ? System.Web.Caching.Cache.NoAbsoluteExpiration
                : Platform.Time.Add(expiryTime);
            TimeSpan slidingExpiration = sliding ? expiryTime
                : System.Web.Caching.Cache.NoSlidingExpiration;

            _cache.Insert(
                qualifiedKey,
                new DictionaryEntry(key, value),
                dependency,
                absExpiration,
                slidingExpiration,
                System.Web.Caching.CacheItemPriority.Normal,
                OnCacheItemRemoved);
        }

        private void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            // TODO logging
        }

        #endregion

    }
}
