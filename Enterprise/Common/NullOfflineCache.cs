#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Null implementation of <see cref="IOfflineCache{TKey,TValue}"/>.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	class NullOfflineCache<TKey, TValue> : IOfflineCache<TKey, TValue>, IOfflineCacheClient<TKey, TValue>
	{
		#region Implementation of IOfflineCache

		public IOfflineCacheClient<TKey, TValue> CreateClient()
		{
			return this;
		}

		#endregion

		#region Implementation of IOfflineCacheClient

		public TValue Get(TKey key)
		{
			return default(TValue);
		}

		public void Put(TKey key, TValue value)
		{
		}

		public void Remove(TKey key)
		{
		}

		#endregion

		#region Implementation of IDisposable

		public void Dispose()
		{
		}

		#endregion
	}
}
