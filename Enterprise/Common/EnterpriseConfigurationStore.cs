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
            string content = null;
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    content = service.GetConfigurationDocument(
						new GetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(name, version, user, instanceKey))).Content;
                });

            if(content == null)
                throw new ConfigurationDocumentNotFoundException(name, version, user, instanceKey);

            return new StringReader(content);
        }

        /// <summary>
        /// Stores the specified document for the current user and instance key.  If user is null,
        /// the document is stored as a shared document.
        /// </summary>
        public void PutDocument(string name, Version version, string user, string instanceKey, TextReader content)
        {
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    service.SetConfigurationDocument(
						new SetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(name, version, user, instanceKey), content.ReadToEnd()));
                });
        }

        #endregion
    }
}
