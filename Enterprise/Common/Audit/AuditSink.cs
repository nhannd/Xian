#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.Audit
{
	[ExtensionPoint]
	public class EnterpriseAuditSinkOfflineCacheExtensionPoint : ExtensionPoint<IOfflineCache<Guid, AuditEntryInfo>>
	{
	}

	/// <summary>
	/// An implementation of <see cref="IAuditSink"/> that sinks to the <see cref="IAuditService"/>.
	/// </summary>
	[ExtensionOf(typeof(AuditSinkExtensionPoint))]
	public class AuditSink : IAuditSink
	{
		private readonly IOfflineCache<Guid, AuditEntryInfo> _offlineCache;

		public AuditSink()
		{
			try
			{
				_offlineCache = (IOfflineCache<Guid, AuditEntryInfo>)(new EnterpriseAuditSinkOfflineCacheExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, SR.ExceptionOfflineCacheNotFound);
			}
		}

		#region IAuditSink Members

		/// <summary>
		/// Writes the specified entry to the sink.
		/// </summary>
		/// <param name="entry"></param>
		public void WriteEntry(AuditEntryInfo entry)
		{
			try
			{
				Platform.GetService<IAuditService>(service => service.WriteEntry(new WriteEntryRequest(entry)));
			}
			catch (EndpointNotFoundException e)
			{
				if(_offlineCache == null)
					throw new AuditException(SR.ExceptionAuditServiceNotReachableAndNoOfflineCache, e);

				StoreOffline(entry);
			}
		}

		#endregion

		private void StoreOffline(AuditEntryInfo entry)
		{
			try
			{
				using (var client = _offlineCache.CreateClient())
				{
					// stick it in the offline cache
					// any unique value can be used as a key, because it will never be accessed by key again
					client.Put(Guid.NewGuid(), entry);
				}
			}
			catch (Exception e)
			{
				throw new AuditException(SR.ExceptionAuditServiceNotReachableAndNoOfflineCache, e);
			}
		}
	}
}
