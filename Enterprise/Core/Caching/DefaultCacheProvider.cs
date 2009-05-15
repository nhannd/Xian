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
	[ExtensionOf(typeof(CacheClientProviderExtensionPoint))]
	public class DefaultCacheProvider : ICacheClientProvider
	{
		private static readonly string _cacheKeyPrefix = Guid.NewGuid().ToString("N");
		private readonly Dictionary<string, string> _rootCacheKeys = new Dictionary<string, string>();
		private Cache _cache;

		#region ICacheClientProvider Members

		public void Initialize(CacheClientProviderInitializationArgs args)
		{
			_cache = HttpRuntime.Cache;
		}

		public ICacheClient CreateCacheClient(CacheClientCreationArgs args)
		{
			string rootKey;
			if(!_rootCacheKeys.TryGetValue(args.Region, out rootKey))
			{
				rootKey = GetQualifiedCacheKey(args.Region, Guid.NewGuid().ToString("N"));
				StoreRootCacheKey(rootKey);
				_rootCacheKeys.Add(args.Region, rootKey);
			}

			return new DefaultCacheClient(this, args);
		}

		#endregion

		internal object Get(string region, string key)
		{
			Platform.CheckForNullReference(key, "key");

			string cacheKey = GetQualifiedCacheKey(region, key);

			object obj = _cache.Get(cacheKey);
			if (obj == null)
				return null;

			DictionaryEntry entry = (DictionaryEntry)obj;
			return key.Equals(entry.Key) ? entry.Value : null;
		}

		internal void Put(string region, string key, object value, TimeSpan expiration)
		{
			Platform.CheckForNullReference(key, "key");
			Platform.CheckForNullReference(value, "value");

			string qualifiedKey = GetQualifiedCacheKey(region, key);
			if (_cache[qualifiedKey] != null)
			{
				// Remove the key to re-add it again below
				_cache.Remove(qualifiedKey);
			}

			_cache.Add(
				qualifiedKey,
				new DictionaryEntry(key, value),
				new CacheDependency(null, new string[] { _rootCacheKeys[region] }),
				Platform.Time.Add(expiration),
				Cache.NoSlidingExpiration,
				CacheItemPriority.Normal,
				null);
		}

		internal void Remove(string region, string key)
		{
			Platform.CheckForNullReference(key, "key");
			string qualifiedKey = GetQualifiedCacheKey(region, key);
			_cache.Remove(qualifiedKey);
		}

		internal void Clear(string region)
		{
			string rootKey = _rootCacheKeys[region];
			_cache.Remove(rootKey);
			StoreRootCacheKey(rootKey);
		}

		private void StoreRootCacheKey(string rootKey)
		{
			// remove the root key in case already stored (note, this will clear the 
			_cache.Remove(rootKey);
			_cache.Add(
				rootKey,
				rootKey,
				null,
				Cache.NoAbsoluteExpiration,
				Cache.NoSlidingExpiration,
				CacheItemPriority.NotRemovable,
				null);
		}

		private static string GetQualifiedCacheKey(string region, string key)
		{
			return string.Format("{0}:{1}:{2}", _cacheKeyPrefix, region, key);
		}
	}
}
