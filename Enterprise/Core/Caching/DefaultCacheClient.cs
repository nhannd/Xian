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
		private readonly string _region;
		private readonly DefaultCacheProvider _provider;
		private readonly TimeSpan _expiration;

		internal DefaultCacheClient(DefaultCacheProvider provider, CacheClientCreationArgs args)
		{
			_provider = provider;
			_region = StringUtilities.EmptyIfNull(args.Region);
			_expiration = args.ExpirationTime;
		}

		#region ICacheClient Members

		public object Get(string key)
		{
			return _provider.Get(_region, key);
		}

		public void Put(string key, object value)
		{
			_provider.Put(_region, key, value, _expiration);
		}

		public void Remove(string key)
		{
			_provider.Remove(_region, key);
		}

		public void Clear()
		{
			_provider.Clear(_region);
		}

		public string RegionName
		{
			get { return _region; }
		}

		#endregion
	}
}
