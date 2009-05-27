using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.Caching
{
	internal class CacheClientLoggingDecorator : ICacheClient
	{
		private readonly ICacheClient _cacheClient;
		private readonly LogLevel _logLevel;

		public CacheClientLoggingDecorator(ICacheClient cacheClient, LogLevel logLevel)
		{
			_cacheClient = cacheClient;
			_logLevel = logLevel;
		}


		#region ICacheClient Members

		public string CacheID
		{
			get { return _cacheClient.CacheID; }
		}

		public object Get(string key, CacheGetOptions options)
		{
			object value = _cacheClient.Get(key, options);
			LogGet(key, value, options);
			return value;
		}

		public void Put(string key, object value, CachePutOptions options)
		{
			_cacheClient.Put(key, value, options);
			LogPut(key, value, options);
		}

		public void Remove(string key, CacheRemoveOptions options)
		{
			_cacheClient.Remove(key, options);
			LogRemove(key, options);
		}

		public bool RegionExists(string region)
		{
			return _cacheClient.RegionExists(region);
		}

		public void ClearRegion(string region)
		{
			_cacheClient.ClearRegion(region);
			LogClearRegion(region);
		}

		public void ClearCache()
		{
			_cacheClient.ClearCache();
			LogClearCache();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_cacheClient.Dispose();
		}

		#endregion

		private void LogGet(string key, object value, CacheGetOptions options)
		{
			string action = value == null ? "miss" : "hit";
			LogItem(action, options.Region, key, value);
		}

		private void LogPut(string key, object value, CachePutOptions options)
		{
			LogItem("put", options.Region, key, value);
		}

		private void LogRemove(string key, CacheRemoveOptions options)
		{
			LogItem("remove", options.Region, key, null);
		}

		private void LogClearRegion(string region)
		{
			Platform.Log(_logLevel,
			             "Cache (ID = {0}, Region = {1}): cleared region",
			             _cacheClient.CacheID,
			             region);
		}

		private void LogClearCache()
		{
			Platform.Log(_logLevel,
			             "Cache (ID = {0}): cleared cache",
			             _cacheClient.CacheID);
		}

		private void LogItem(string action, string region, string key, object value)
		{
			Platform.Log(_logLevel,
				"Cache (ID = {0}, Region = {1}): {2} key = {3}, value = {4}",
				_cacheClient.CacheID,
				region,
				action,
				key,
				value);
		}
	}
}
