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
        private readonly string _cacheID;
		private readonly string _region;
		private readonly DefaultCacheProvider _provider;
		private readonly TimeSpan _expiration;
        private readonly bool _sliding;

		internal DefaultCacheClient(DefaultCacheProvider provider, CacheClientCreationArgs args)
		{
			_provider = provider;
            _cacheID = args.CacheID;
			_region = StringUtilities.EmptyIfNull(args.Region);
			_expiration = args.ExpirationTime;
            _sliding = args.SlidingExpiration;
		}

		#region ICacheClient Members

        public string CacheID
        {
            get { return _cacheID; }
        }

        public string Region
        {
            get { return _region; }
        }

        public object Get(string key)
		{
            return _provider.Get(_cacheID, _region, key);
		}

		public void Put(string key, object value)
		{
			_provider.Put(_cacheID, _region, key, value, _expiration, _sliding);
		}

        public void Put(string key, object value, TimeSpan expiration, bool sliding)
        {
            _provider.Put(_cacheID, _region, key, value, expiration, sliding);
        }

		public void Remove(string key)
		{
            _provider.Remove(_cacheID, _region, key);
		}

		public void ClearRegion()
		{
            _provider.ClearRegion(_cacheID, _region);
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
