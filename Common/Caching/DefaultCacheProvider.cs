#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace ClearCanvas.Common.Caching
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
		public ICacheClient CreateClient(string cacheId)
		{
			// ensure cache exists
			CreateCache(cacheId);

			return new DefaultCacheClient(this, cacheId);
		}

		#endregion

		#region Internal API

		internal object Get(string cacheId, string key, CacheGetOptions options)
		{
			Platform.CheckForNullReference(key, "key");
			Platform.CheckForNullReference(options, "options");

			var cacheKey = GetItemCacheKey(cacheId, options.Region, key);

			var obj = _cache.Get(cacheKey);
			if (obj == null)
				return null;

			var entry = (DictionaryEntry)obj;
			return key.Equals(entry.Key) ? entry.Value : null;
		}

		internal void Put(string cacheId, string key, object value, CachePutOptions options)
		{
			Platform.CheckForNullReference(key, "key");
			Platform.CheckForNullReference(value, "value");
			Platform.CheckForNullReference(options, "options");

			// ensure region exists
			CreateRegion(cacheId, options.Region);

			var cacheKey = GetItemCacheKey(cacheId, options.Region, key);
			PutItem(cacheKey, key, GetRegionCacheKey(cacheId, options.Region),
				value, options.Expiration, options.Sliding);
		}


		internal void Remove(string cacheId, string key, CacheRemoveOptions options)
		{
			Platform.CheckForNullReference(key, "key");
			Platform.CheckForNullReference(options, "options");

			var cacheKey = GetItemCacheKey(cacheId, options.Region, key);
			_cache.Remove(cacheKey);
		}

		internal bool RegionExists(string cacheId, string region)
		{
			var regionKey = GetRegionCacheKey(cacheId, region);
			return _cache.Get(regionKey) != null;
		}

		internal void ClearRegion(string cacheId, string region)
		{
			var regionKey = GetRegionCacheKey(cacheId, region);

			// remove region key to clear all items
			_cache.Remove(regionKey);

			// re-create region
			CreateRegion(cacheId, regionKey);
		}

		internal void ClearCache(string cacheId)
		{
			var rootKey = GetRootCacheKey(cacheId);

			// remove root key to clear all items
			_cache.Remove(rootKey);

			// re-create cache
			CreateCache(cacheId);
		}

		#endregion

		#region Helpers

		private void CreateCache(string cacheId)
		{
			CreateRoot(GetRootCacheKey(cacheId), null);
		}

		private void CreateRegion(string cacheId, string region)
		{
			CreateRoot(GetRegionCacheKey(cacheId, region), GetRootCacheKey(cacheId));
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
					dependencyKey == null ? null : new CacheDependency(null, new [] { dependencyKey }),
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
			var dependency = dependencyKey == null ? null
				: new CacheDependency(null, new [] { dependencyKey });

			var absExpiration = sliding ? System.Web.Caching.Cache.NoAbsoluteExpiration
				: Platform.Time.Add(expiryTime);
			var slidingExpiration = sliding ? expiryTime
				: System.Web.Caching.Cache.NoSlidingExpiration;

			_cache.Insert(
				qualifiedKey,
				new DictionaryEntry(key, value),
				dependency,
				absExpiration,
				slidingExpiration,
				CacheItemPriority.Normal,
				OnCacheItemRemoved);
		}

		private void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
		{
			// TODO logging
		}

		#endregion


	}
}
