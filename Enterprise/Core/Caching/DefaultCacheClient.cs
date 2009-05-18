using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ClearCanvas.Enterprise.Common.Caching;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core.Caching
{
	internal class DefaultCacheClient : ICacheClient
	{
        private readonly DefaultCacheProvider _provider;
        private readonly string _cacheID;

		internal DefaultCacheClient(DefaultCacheProvider provider, string cacheID)
		{
			_provider = provider;
            _cacheID = cacheID;
		}

		#region ICacheClient Members

        public string CacheID
        {
            get { return _cacheID; }
        }

        public object Get(string key, CacheGetOptions options)
		{
            return _provider.Get(_cacheID, key, options);
		}

		public void Put(string key, object value, CachePutOptions options)
		{
			_provider.Put(_cacheID, key, value, options);
		}

		public void Remove(string key, CacheRemoveOptions options)
		{
            _provider.Remove(_cacheID, key, options);
		}

        public bool RegionExists(string region)
        {
            return _provider.RegionExists(_cacheID, region);
        }

		public void ClearRegion(string region)
		{
            _provider.ClearRegion(_cacheID, region);
		}

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
