#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Caching;
using ClearCanvas.Enterprise.Common.Configuration;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionPoint]
	public class EnterpriseConfigurationStoreOfflineCacheExtensionPoint : ExtensionPoint<IOfflineCache<ConfigurationDocumentKey, string>>
	{
	}

	/// <summary>
	/// Enterprise implementation of <see cref="IConfigurationStore"/>.
	/// </summary>
	[ExtensionOf(typeof(ConfigurationStoreExtensionPoint))]
	public class EnterpriseConfigurationStore : IConfigurationStore
	{
		private readonly IOfflineCache<ConfigurationDocumentKey, string> _offlineCache;

		public EnterpriseConfigurationStore()
		{
			try
			{
				_offlineCache = (IOfflineCache<ConfigurationDocumentKey, string>)(new EnterpriseConfigurationStoreOfflineCacheExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, SR.ExceptionOfflineCacheNotFound);
				_offlineCache = new NullOfflineCache<ConfigurationDocumentKey, string>();
			}
		}

		#region IConfigurationStore Members

		/// <summary>
		/// Obtains the specified document for the specified user and instance key.  If user is null,
		/// the shared document is obtained.
		/// </summary>
		public TextReader GetDocument(string name, Version version, string user, string instanceKey)
		{
			// choose the anonymous-access service if possible
			var serviceContract = string.IsNullOrEmpty(user) ?
				typeof(IApplicationConfigurationReadService) : typeof(IConfigurationService);

			var service = (IApplicationConfigurationReadService)Platform.GetService(serviceContract);
			using (service as IDisposable)
			using (var offlineCacheClient = _offlineCache.CreateClient())
			{
				var documentKey = new ConfigurationDocumentKey(name, version, user, instanceKey);
				string content;
				try
				{
					content = service.GetConfigurationDocument(new GetConfigurationDocumentRequest(documentKey)).Content;

					// keep offline cache up to date
					offlineCacheClient.Put(documentKey, content);
				}
				catch (EndpointNotFoundException e)
				{
					Platform.Log(LogLevel.Debug, e);

					// get most recent version from offline cache
					content = offlineCacheClient.Get(documentKey);
				}

				if (content == null)
					throw new ConfigurationDocumentNotFoundException(name, version, user, instanceKey);

				return new StringReader(content);
			}
		}

		/// <summary>
		/// Stores the specified document for the current user and instance key.  If user is null,
		/// the document is stored as a shared document.
		/// </summary>
		public void PutDocument(string name, Version version, string user, string instanceKey, TextReader content)
		{
			using (var offlineCacheClient = _offlineCache.CreateClient())
			{
				var documentKey = new ConfigurationDocumentKey(name, version, user, instanceKey);
				var document = content.ReadToEnd();
				Platform.GetService<IConfigurationService>(
						service => service.SetConfigurationDocument(
								new SetConfigurationDocumentRequest(documentKey, document))
					);

				// note: we don't catch exceptions here.  If we're offline, the app should not
				// be allowing the user to attempt to put documents to the server, hence this
				// method shouldn't even be called

				// keep offline cache up to date
				offlineCacheClient.Put(documentKey, document);
			}
		}

		#endregion
	}
}
