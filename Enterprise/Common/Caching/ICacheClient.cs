using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	public interface ICacheClient
	{
		/// <summary>
		/// Gets the object from the cache.
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
		/// Removes the specified item from the cache.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <exception cref="CacheException"></exception>
		void Remove(string key);

		/// <summary>
		/// Clears the cache.
		/// </summary>
		/// <exception cref="CacheException"></exception>
		void Clear();

		/// <summary>
		/// Gets the name of the cache region.
		/// </summary>
		string RegionName { get; }
	}
}
