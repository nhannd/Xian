#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Caching
{
	/// <summary>
	/// Implementation of <see cref="ICacheClient"/> for <see cref="DefaultCacheProvider"/>.
	/// </summary>
	internal class DefaultCacheClient : ICacheClient
	{
		private readonly DefaultCacheProvider _provider;
		private readonly string _cacheId;

		/// <summary>
		/// Internal constructor.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="cacheId"></param>
		internal DefaultCacheClient(DefaultCacheProvider provider, string cacheId)
		{
			_provider = provider;
			_cacheId = cacheId;
		}

		#region ICacheClient Members

		/// <summary>
		/// Gets the ID of the logical cache that this client is connected to.
		/// </summary>
		public string CacheID
		{
			get { return _cacheId; }
		}

		/// <summary>
		/// Gets the object at the specified key from the cache, or null if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public object Get(string key, CacheGetOptions options)
		{
			return _provider.Get(_cacheId, key, options);
		}

		/// <summary>
		/// Puts the specified object into the cache at the specified key,
		/// using the specified options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="options"></param>
		public void Put(string key, object value, CachePutOptions options)
		{
			_provider.Put(_cacheId, key, value, options);
		}

		/// <summary>
		/// Removes the specified item from the cache, or does nothing if the item does not
		/// exist.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <param name="options"></param>
		public void Remove(string key, CacheRemoveOptions options)
		{
			_provider.Remove(_cacheId, key, options);
		}

		/// <summary>
		/// Gets a value indicating whether the specified region exists.
		/// </summary>
		/// <param name="region"></param>
		/// <returns></returns>
		public bool RegionExists(string region)
		{
			return _provider.RegionExists(_cacheId, region);
		}

		/// <summary>
		/// Clears the entire cache region.
		/// </summary>
		public void ClearRegion(string region)
		{
			_provider.ClearRegion(_cacheId, region);
		}

		/// <summary>
		/// Clears the entire logical cache (as identified by <see cref="ICacheClient.CacheID"/>.
		/// </summary>
		public void ClearCache()
		{
			_provider.ClearCache(_cacheId);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			// nothing to do
		}

		#endregion
	}
}
