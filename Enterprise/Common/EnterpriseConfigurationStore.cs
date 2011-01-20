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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Configuration;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Enterprise implementation of <see cref="IConfigurationStore"/>.
    /// </summary>
    [ExtensionOf(typeof(ConfigurationStoreExtensionPoint))]
    public class EnterpriseConfigurationStore : IConfigurationStore
    {
        #region IConfigurationStore Members

        /// <summary>
        /// Obtains the specified document for the specified user and instance key.  If user is null,
        /// the shared document is obtained.
        /// </summary>
        public TextReader GetDocument(string name, Version version, string user, string instanceKey)
        {
            // choose the anonymous-access service if possible
            Type serviceContract = string.IsNullOrEmpty(user) ?
                typeof(IApplicationConfigurationReadService) : typeof(IConfigurationService);

            IApplicationConfigurationReadService service = (IApplicationConfigurationReadService)Platform.GetService(serviceContract);
            using (service as IDisposable)
            {
                string content = service.GetConfigurationDocument(
                   new GetConfigurationDocumentRequest(
                       new ConfigurationDocumentKey(name, version, user, instanceKey))).Content;
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
			Platform.GetService<Configuration.IConfigurationService>(
				delegate(Configuration.IConfigurationService service)
                {
                    service.SetConfigurationDocument(
						new SetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(name, version, user, instanceKey), content.ReadToEnd()));
                });
        }

        #endregion
    }
}
