#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Configuration;

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
		#region Document helper class

		class Document : IConfigurationDocument
		{
			private readonly GetConfigurationDocumentResponse _response;
			private ConfigurationDocumentHeader _header;

			internal Document(GetConfigurationDocumentResponse response)
			{
				_response = response;
			}

			#region Implementation of IConfigurationDocument

			public ConfigurationDocumentHeader Header
			{
				get
				{
					if(_header == null)
					{
						// todo date values
						_header = new ConfigurationDocumentHeader(_response.DocumentKey, DateTime.MinValue, DateTime.MinValue);
					}
					return _header;
				}
			}

			public string ReadAll()
			{
				return _response.Content;
			}

			public TextReader GetReader()
			{
				return new StringReader(_response.Content);
			}

			#endregion
		}

		#endregion

		#region Implementation of IConfigurationStore

		public IEnumerable<ConfigurationDocumentHeader> ListDocuments(ConfigurationDocumentQuery query)
		{
			// choose the anonymous-access service if possible (if we're not querying for user docs)
			var serviceContract = query.UserType == ConfigurationDocumentQuery.DocumentUserType.Shared ?
				typeof(IApplicationConfigurationReadService) : typeof(IConfigurationService);

			var service = (IApplicationConfigurationReadService)Platform.GetService(serviceContract);
			using (service as IDisposable)
			{
				var response = service.ListConfigurationDocuments(new ListConfigurationDocumentsRequest(query));
				return response.Documents;
			}
		}

		public IConfigurationDocument GetDocument(ConfigurationDocumentKey documentKey)
		{
			// choose the anonymous-access service if possible
			var serviceContract = string.IsNullOrEmpty(documentKey.User) ?
				typeof(IApplicationConfigurationReadService) : typeof(IConfigurationService);

			var service = (IApplicationConfigurationReadService)Platform.GetService(serviceContract);
			using (service as IDisposable)
			{
				var response = service.GetConfigurationDocument(new GetConfigurationDocumentRequest(documentKey));
				if (response.Content == null)
					throw new ConfigurationDocumentNotFoundException(documentKey);

				return new Document(response);
			}
		}

		public void PutDocument(ConfigurationDocumentKey documentKey, TextReader content)
		{
			PutDocument(documentKey, content.ReadToEnd());
		}

		public void PutDocument(ConfigurationDocumentKey documentKey, string content)
		{
			Platform.GetService<IConfigurationService>(
					service => service.SetConfigurationDocument(
							new SetConfigurationDocumentRequest(documentKey, content))
				);
		}

		#endregion
	}
}
