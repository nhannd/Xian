using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
    /// <summary>
    /// Defines an interface to an object that acts as a client of a cache.
    /// </summary>
	public interface ICacheClient : IDisposable
	{
        /// <summary>
        /// Gets the ID of the logical cache that this client is connected to.
        /// </summary>
        string CacheID { get; }

        /// <summary>
		/// Gets the object at the specified key from the cache, or null if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		object Get(string key, CacheGetOptions options);

        /// <summary>
        /// Puts the specified object into the cache at the specified key,
        /// using the specified options.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        void Put(string key, object value, CachePutOptions options);

		/// <summary>
		/// Removes the specified item from the cache, or does nothing if the item does not
        /// exist.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <param name="options"></param>
		void Remove(string key, CacheRemoveOptions options);

        /// <summary>
        /// Gets a value indicating whether the specified region exists.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        bool RegionExists(string region);

        /// <summary>
        /// Clears the entire cache region.
        /// </summary>
        void ClearRegion(string region);

		/// <summary>
		/// Clears the entire logical cache (as identified by <see cref="CacheID"/>.
		/// </summary>
		void ClearCache();
	}
}
