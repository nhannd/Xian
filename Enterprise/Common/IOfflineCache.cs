#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines an interface to a cache used for offline storage of enterprise data.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public interface IOfflineCache<TKey, TValue>
	{
		/// <summary>
		/// Creates a client for accessing this offline cache, for use by a single thread.
		/// </summary>
		/// <remarks>
		/// Implementations of this method must be safe for concurrent use by multiple threads.
		/// However, the returned object will only ever be used by a single thread, and therefore
		/// needn't be thread-safe.
		/// </remarks>
		/// <returns></returns>
		IOfflineCacheClient<TKey, TValue> CreateClient();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Safe for use by a single thread only.
	/// </remarks>
	public interface IOfflineCacheClient<TKey, TValue> : IDisposable
	{
		/// <summary>
		/// Gets the value at the specified key from the cache.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		TValue Get(TKey key);

		/// <summary>
		/// Puts the specified value into the cache at the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void Put(TKey key, TValue value);

		/// <summary>
		/// Removes the specified key and corresponding value from the cache.
		/// </summary>
		/// <param name="key"></param>
		void Remove(TKey key);
	}
}
