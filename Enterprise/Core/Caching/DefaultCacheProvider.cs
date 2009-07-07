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
using System.Collections;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Enterprise.Common.Caching;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core.Caching
{
	/// <summary>
	/// Default implementation of <see cref="ICacheProvider"/>, provides a local in-process cache.
	/// </summary>
	[ExtensionOf(typeof(CacheProviderExtensionPoint))]
	public class DefaultCacheProvider : ICacheProvider
	{
		private System.Web.Caching.Cache _cache;

		#region ICacheProvider Members

		/// <summary>
		/// Initializes this cache provider.
		/// </summary>
		public void Initialize(CacheProviderInitializationArgs args)
		{
            // This may seem odd, but using the ASP.NET cache outside of an ASP app
            // is perfectly ok, according to this MSDN article:
            // http://msdn.microsoft.com/en-us/library/ms978500.aspx
			_cache = HttpRuntime.Cache;
		}

		/// <summary>
		/// Creates a cache client for the specified logical cache ID.
		/// </summary>
		/// <remarks>
		/// The implementation of this method *must* be safe for multiple threads making concurrent calls.
		/// </remarks>
		/// <returns></returns>
		public ICacheClient CreateClient(string cacheID)
		{
            // ensure cache exists
            CreateCache(cacheID);

            return new DefaultCacheClient(this, cacheID);
		}

		#endregion

        #region Internal API

        internal object Get(string cacheID, string key, CacheGetOptions options)
		{
			Platform.CheckForNullReference(key, "key");
            Platform.CheckForNullReference(options, "options");

            string cacheKey = GetItemCacheKey(cacheID, options.Region, key);

			object obj = _cache.Get(cacheKey);
			if (obj == null)
				return null;

			DictionaryEntry entry = (DictionaryEntry)obj;
			return key.Equals(entry.Key) ? entry.Value : null;
		}

        internal void Put(string cacheID, string key, object value, CachePutOptions options)
		{
			Platform.CheckForNullReference(key, "key");
			Platform.CheckForNullReference(value, "value");
            Platform.CheckForNullReference(options, "options");

            // ensure region exists
            CreateRegion(cacheID, options.Region);

            string cacheKey = GetItemCacheKey(cacheID, options.Region, key);
            PutItem(cacheKey, key, GetRegionCacheKey(cacheID, options.Region),
                value, options.Expiration, options.Sliding);
		}


        internal void Remove(string cacheID, string key, CacheRemoveOptions options)
		{
			Platform.CheckForNullReference(key, "key");
            Platform.CheckForNullReference(options, "options");

            string cacheKey = GetItemCacheKey(cacheID, options.Region, key);
            _cache.Remove(cacheKey);
		}

        internal bool RegionExists(string cacheID, string region)
        {
            string regionKey = GetRegionCacheKey(cacheID, region);
            return _cache.Get(regionKey) != null;
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
                // add root key, dependent on dependencyKey
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
