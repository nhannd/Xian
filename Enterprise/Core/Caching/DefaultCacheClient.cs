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
using ClearCanvas.Enterprise.Common.Caching;

namespace ClearCanvas.Enterprise.Core.Caching
{
	/// <summary>
	/// Implementation of <see cref="ICacheClient"/> for <see cref="DefaultCacheProvider"/>.
	/// </summary>
	internal class DefaultCacheClient : ICacheClient
	{
        private readonly DefaultCacheProvider _provider;
        private readonly string _cacheID;

		/// <summary>
		/// Internal constructor.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="cacheID"></param>
		internal DefaultCacheClient(DefaultCacheProvider provider, string cacheID)
		{
			_provider = provider;
            _cacheID = cacheID;
		}

		#region ICacheClient Members

		/// <summary>
		/// Gets the ID of the logical cache that this client is connected to.
		/// </summary>
		public string CacheID
        {
            get { return _cacheID; }
        }

		/// <summary>
		/// Gets the object at the specified key from the cache, or null if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public object Get(string key, CacheGetOptions options)
		{
            return _provider.Get(_cacheID, key, options);
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
			_provider.Put(_cacheID, key, value, options);
		}

		/// <summary>
		/// Removes the specified item from the cache, or does nothing if the item does not
		/// exist.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <param name="options"></param>
		public void Remove(string key, CacheRemoveOptions options)
		{
            _provider.Remove(_cacheID, key, options);
		}

		/// <summary>
		/// Gets a value indicating whether the specified region exists.
		/// </summary>
		/// <param name="region"></param>
		/// <returns></returns>
		public bool RegionExists(string region)
        {
            return _provider.RegionExists(_cacheID, region);
        }

		/// <summary>
		/// Clears the entire cache region.
		/// </summary>
		public void ClearRegion(string region)
		{
            _provider.ClearRegion(_cacheID, region);
		}

		/// <summary>
		/// Clears the entire logical cache (as identified by <see cref="ICacheClient.CacheID"/>.
		/// </summary>
		public void ClearCache()
        {
            _provider.ClearCache(_cacheID);
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
