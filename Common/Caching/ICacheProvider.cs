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
    /// Defines an interfaces to an object that provides a cache implementation.
    /// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// Initializes this cache provider.
		/// </summary>
		void Initialize(CacheProviderInitializationArgs args);


		/// <summary>
		/// Creates a cache client for the specified logical cache ID.
		/// </summary>
        /// <remarks>
        /// The implementation of this method *must* be safe for multiple threads making concurrent calls.
        /// </remarks>
		/// <returns></returns>
		ICacheClient CreateClient(string cacheID);
	}
}
