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
        /// Gets the cache region associated with this client.
        /// </summary>
        string Region { get; }

        /// <summary>
		/// Gets the object at the specified key from the cache, or null if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="CacheException"></exception>
		object Get(string key);

		/// <summary>
		/// Puts the specified object into the cache at the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <exception cref="CacheException"></exception>
		void Put(string key, object value);

		/// <summary>
		/// Removes the specified item from the cache, or does nothing if the item does not
        /// exist.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <exception cref="CacheException"></exception>
		void Remove(string key);

        /// <summary>
        /// Clears the entire cache region (as identified by <see cref="Region"/>).
        /// </summary>
        void ClearRegion();

		/// <summary>
		/// Clears the entire logical cache (as identified by <see cref="CacheID"/>.
		/// </summary>
		/// <exception cref="CacheException"></exception>
		void ClearCache();
	}
}
