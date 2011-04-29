#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Time;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionPoint]
	public class EnterpriseTimeProviderOfflineCacheExtensionPoint : ExtensionPoint<IOfflineCache<string, string>>
	{
	}

	/// <summary>
	/// Provides a consistent time to the application from the enterprise server.
	/// </summary>
	[ExtensionOf(typeof(TimeProviderExtensionPoint))]
	public class EnterpriseTimeProvider : ITimeProvider
	{
		// this key could be anything, as long as it's unique
		private const string TimeOffsetCacheKey = "{92E55B13-96A5-4a03-A669-B22D5D29E95B}";

		private TimeSpan _localToEnterpriseOffset;
		private DateTime _lastResyncInLocalTime;
		private DateTime _lastSyncTime;
		private readonly TimeSpan _resyncPeriod;
		private readonly TimeSpan _maxTimeBetweenSync;
		private readonly IOfflineCache<string, string> _offlineCache;

		public EnterpriseTimeProvider()
		{
			_lastResyncInLocalTime = DateTime.MinValue;
			_resyncPeriod = new TimeSpan(0, 0, 60);
			_maxTimeBetweenSync = new TimeSpan(0, 10, 0);

			try
			{
				_offlineCache = (IOfflineCache<string, string>)(new EnterpriseTimeProviderOfflineCacheExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, SR.ExceptionOfflineCacheNotFound);

				_offlineCache = new NullOfflineCache<string, string>();
			}
		}

		#region ITimeProvider Members

		public DateTime CurrentTime
		{
			get
			{
				if (ResyncRequired())
				{
					ResyncLocalToEnterpriseTime();
				}
				return EnterpriseTimeFromLocal(DateTime.Now);
			}
		}

		#endregion

		private DateTime EnterpriseTimeFromLocal(DateTime localTime)
		{
			return localTime - _localToEnterpriseOffset;
		}

		private bool ResyncRequired()
		{
			return (DateTime.Now - _lastResyncInLocalTime) > _resyncPeriod;
		}

		private void ResyncLocalToEnterpriseTime()
		{
			using (var client = _offlineCache.CreateClient())
			{
				try
				{
					var eTime = CurrentEnterpriseTime();
					_lastSyncTime = DateTime.Now;
					_localToEnterpriseOffset = _lastSyncTime - eTime;

					// update offline cache
					client.Put(TimeOffsetCacheKey, _localToEnterpriseOffset.TotalMilliseconds.ToString());
				}
				catch (Exception)
				{
					if ((DateTime.Now - _lastSyncTime) > _maxTimeBetweenSync)
						Platform.Log(LogLevel.Warn, "Unable to contact time server for synchronization");

					// if the process has just started up, and we have not yet been able to connect to the server,
					// attempt to read last known value from the offline cache
					if (_localToEnterpriseOffset == TimeSpan.Zero)
					{
						var s = client.Get(TimeOffsetCacheKey);
						_localToEnterpriseOffset = (s == null) ? TimeSpan.Zero : TimeSpan.FromMilliseconds(double.Parse(s));
					}
				}
			}
		}

		private DateTime CurrentEnterpriseTime()
		{
			var time = default(DateTime);

			Platform.GetService(
				delegate(ITimeService service)
				{
					time = service.GetTime(new GetTimeRequest()).Time;
				});

			_lastResyncInLocalTime = DateTime.Now;

			return time;
		}
	}
}
