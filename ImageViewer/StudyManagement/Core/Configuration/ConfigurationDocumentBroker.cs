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
            return GetConfigurationDocument(documentKey, false);
        }

        /// <summary>
        /// Get the specified ConfigurationDocument or null if not found
        /// </summary>
        /// <param name="documentKey"></param>
        /// <returns></returns>
        public ConfigurationDocument GetPriorConfigurationDocument(ConfigurationDocumentKey documentKey)
        {
            return GetConfigurationDocument(documentKey, true);
        }

        private ConfigurationDocument GetConfigurationDocument(ConfigurationDocumentKey documentKey, bool prior)
        {
            IQueryable<ConfigurationDocument> query = from d in Context.ConfigurationDocuments select d;

            query = !string.IsNullOrEmpty(documentKey.InstanceKey) 
                ? query.Where(d => d.InstanceKey == documentKey.InstanceKey)
                : query.Where(d => d.InstanceKey == null);

            query = !string.IsNullOrEmpty(documentKey.User) 
                ? query.Where(d => d.User == documentKey.User) 
                : query.Where(d => d.User == null);

            query = query.Where(d => d.DocumentName == documentKey.DocumentName);

            var paddedVersionString = VersionUtils.ToPaddedVersionString(documentKey.Version, false, false);

            if (prior)
            {
                query = query.Where(d => d.DocumentVersionString.CompareTo(paddedVersionString) < 0);
                //You want the most recent prior version.
                query = query.OrderByDescending(d => d.DocumentVersionString);
            }
            else
            {
                query = query.Where(d => d.DocumentVersionString == paddedVersionString);
            }

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

        internal void DeleteAllDocuments()
        {
            Context.ConfigurationDocuments.DeleteAllOnSubmit(Context.ConfigurationDocuments);
        }
    }
}
