#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration
{
    /// <summary>
    /// Internal configuration document broker.
    /// </summary>
    internal class ConfigurationDocumentBroker : Broker
    {
        internal ConfigurationDocumentBroker(ConfigurationDataContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Get the specified ConfigurationDocument or null if not found
        /// </summary>
        /// <param name="documentKey"></param>
        /// <returns></returns>
        public ConfigurationDocument GetConfigurationDocument(ConfigurationDocumentKey documentKey)
        {
            IQueryable<ConfigurationDocument> query = from d in Context.ConfigurationDocuments select d;

            query = !string.IsNullOrEmpty(documentKey.InstanceKey) 
                ? query.Where(d => d.InstanceKey == documentKey.InstanceKey) 
                : query.Where(d => d.InstanceKey == null);

            query = !string.IsNullOrEmpty(documentKey.User) 
                ? query.Where(d => d.User == documentKey.User) 
                : query.Where(d => d.User == null);

            query = query.Where(d => d.DocumentVersionString == VersionUtils.ToPaddedVersionString(documentKey.Version, false, false));

            query = query.Where(d => d.DocumentName == documentKey.DocumentName);

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Insert a ConfigurationDocument
        /// </summary>
        /// <param name="entity"></param>
        public void AddConfigurationDocument(ConfigurationDocument entity)
        {
            Context.ConfigurationDocuments.InsertOnSubmit(entity);
        }
    }

}
