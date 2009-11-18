#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.Enterprise.Configuration
{
    /// <summary>
    /// Contains code shared by different configuration service implementations.
    /// </summary>
    public abstract class ConfigurationServiceBase : CoreServiceLayer
    {
        /// <summary>
        /// Gets the specified configuration document.
        /// </summary>
        /// <param name="documentKey"></param>
        /// <returns></returns>
        protected GetConfigurationDocumentResponse GetConfigurationDocumentHelper(ConfigurationDocumentKey documentKey)
        {
            CheckReadAccess(documentKey);

            IConfigurationDocumentBroker broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
            ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(documentKey);
            IList<ConfigurationDocument> documents = broker.Find(
                new ConfigurationDocumentSearchCriteria[] { criteria }, new SearchResultPage(0, 1), true);

            ConfigurationDocument document = CollectionUtils.FirstElement(documents);
            return new GetConfigurationDocumentResponse(documentKey, document == null ? null : document.Body.DocumentText);
        }

        protected ConfigurationDocumentSearchCriteria BuildCurrentVersionCriteria(ConfigurationDocumentKey key)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(key);
            criteria.DocumentVersionString.EqualTo(VersionUtils.ToPaddedVersionString(key.Version, false, false));
            return criteria;
        }

        protected ConfigurationDocumentSearchCriteria BuildUnversionedCriteria(ConfigurationDocumentKey key)
        {
            ConfigurationDocumentSearchCriteria criteria = new ConfigurationDocumentSearchCriteria();
            criteria.DocumentName.EqualTo(key.DocumentName);

            if (!string.IsNullOrEmpty(key.InstanceKey))
            {
                criteria.InstanceKey.EqualTo(key.InstanceKey);
            }
            else
            {
                criteria.InstanceKey.IsNull();
            }

            if (!string.IsNullOrEmpty(key.User))
            {
                criteria.User.EqualTo(key.User);
            }
            else
            {
                criteria.User.IsNull();
            }

            return criteria;
        }

        /// <summary>
        /// This method is called automatically by response caching framework
        /// to provide caching directive for configuration documents.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected ResponseCachingDirective GetDocumentCachingDirective(GetConfigurationDocumentRequest request)
        {
            // if the request is for ConfigurationStoreSettings, we cannot try to load 
            // these settings to read the values, or we'll get into an infinite recursion
            // therefore, we assume ConfigurationStoreSettings are simply never cached.
            // a better solution would be to allow each settings group to specify its own
            // cacheability, and store this in the db with the settings meta-data
            // but this is not currently implemented
            if (request.DocumentKey.DocumentName == typeof(ConfigurationStoreSettings).FullName)
            {
                return ResponseCachingDirective.DoNotCacheDirective;
            }

            ConfigurationStoreSettings settings = new ConfigurationStoreSettings();
            return new ResponseCachingDirective(
                settings.ConfigurationCachingEnabled,
                TimeSpan.FromSeconds(settings.ConfigurationCachingTimeToLiveSeconds),
                ResponseCachingSite.Client);
        }

        /// <summary>
        /// This method is called automatically by response caching framework
        /// to provide caching directive for settings meta-data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected ResponseCachingDirective GetSettingsMetadataCachingDirective(object request)
        {
            ConfigurationStoreSettings settings = new ConfigurationStoreSettings();
            return new ResponseCachingDirective(
                settings.SettingsMetadataCachingEnabled,
                TimeSpan.FromSeconds(settings.SettingsMetadataCachingTimeToLiveSeconds),
                ResponseCachingSite.Client);
        }

        protected void CheckReadAccess(ConfigurationDocumentKey key)
        {
            string user = key.User;
            if (string.IsNullOrEmpty(user))
            {
                // all users can read application configuration docs
            }
            else
            {
                // user can only read their own configuration docs
                if (user != Thread.CurrentPrincipal.Identity.Name)
                    ThrowNotAuthorized();
            }
        }

        protected void CheckWriteAccess(ConfigurationDocumentKey key)
        {
            string user = key.User;
            if (string.IsNullOrEmpty(user))
            {
                // this is an application configuration doc - need admin permission
                if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.System.Configuration))
                    ThrowNotAuthorized();
            }
            else
            {
                // user can only save their own configuration docs
                if (user != Thread.CurrentPrincipal.Identity.Name)
                    ThrowNotAuthorized();
            }
        }

        protected static void ThrowNotAuthorized()
        {
            throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
        }
    }
}
