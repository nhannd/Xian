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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Enterprise.Core;
using System.Threading;
using ClearCanvas.Common.Configuration;
using IConfigurationService=ClearCanvas.Enterprise.Common.Configuration.IConfigurationService;

namespace ClearCanvas.Enterprise.Configuration
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IConfigurationService))]
    public class ConfigurationService : CoreServiceLayer, IConfigurationService
    {
        #region IConfigurationService Members

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetSettingsMetadataCachingDirective")]
        public ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request)
        {
        	return new ListSettingsGroupsResponse(SettingsGroupDescriptor.ListInstalledSettingsGroups(true));
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetSettingsMetadataCachingDirective")]
        public ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Group, "Group");

        	return new ListSettingsPropertiesResponse(
				SettingsPropertyDescriptor.ListSettingsProperties(request.Group));
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetDocumentCachingDirective")]
        public GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DocumentKey, "DocumentKey");
			
			CheckReadAccess(request.DocumentKey.User);

            IConfigurationDocumentBroker broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
            ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(request.DocumentKey);
            IList<ConfigurationDocument> documents = broker.Find(
                new ConfigurationDocumentSearchCriteria[] { criteria }, new SearchResultPage(0, 1), true);

            ConfigurationDocument document = CollectionUtils.FirstElement(documents);
            return new GetConfigurationDocumentResponse(request.DocumentKey, document == null ? null : document.Body.DocumentText);
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [Audit(typeof(ConfigurationServiceRecorder))]
		public SetConfigurationDocumentResponse SetConfigurationDocument(SetConfigurationDocumentRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DocumentKey, "DocumentKey");

			CheckWriteAccess(request.DocumentKey.User);

            IConfigurationDocumentBroker broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
            ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(request.DocumentKey);
            IList<ConfigurationDocument> documents = broker.Find(
                new ConfigurationDocumentSearchCriteria[] { criteria }, new SearchResultPage(0, 1), true);

            ConfigurationDocument document = CollectionUtils.FirstElement(documents);
            if(document != null)
            {
                document.Body.DocumentText = request.Content;
            }
            else
            {
                // no saved document, create new
                document = NewDocument(request.DocumentKey);
                document.Body.DocumentText = request.Content;
                PersistenceContext.Lock(document, DirtyState.New);
            }

			return new SetConfigurationDocumentResponse();
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
		public RemoveConfigurationDocumentResponse RemoveConfigurationDocument(RemoveConfigurationDocumentRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DocumentKey, "DocumentKey");

			CheckWriteAccess(request.DocumentKey.User);
   
            try
            {
                IConfigurationDocumentBroker broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
                ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(request.DocumentKey);
                ConfigurationDocument document = broker.FindOne(criteria);
                broker.Delete(document);
            }
            catch (EntityNotFoundException)
            {
                // no document - nothing to remove
            }

			return new RemoveConfigurationDocumentResponse();
        }

        #endregion

        /// <summary>
        /// This method is called automatically by response caching framework
        /// to provide caching directive for configuration documents.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private ResponseCachingDirective GetDocumentCachingDirective(GetConfigurationDocumentRequest request)
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
        private ResponseCachingDirective GetSettingsMetadataCachingDirective(object request)
        {
            ConfigurationStoreSettings settings = new ConfigurationStoreSettings();
            return new ResponseCachingDirective(
                settings.SettingsMetadataCachingEnabled,
                TimeSpan.FromSeconds(settings.SettingsMetadataCachingTimeToLiveSeconds),
                ResponseCachingSite.Client);
        }

        private void CheckReadAccess(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                // all users can read application configuration docs
            }
            else
            {
                // user can only read their own configuration docs
                if (user != Thread.CurrentPrincipal.Identity.Name)
                    throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
            }
        }

        private void CheckWriteAccess(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                // this is an application configuration doc - need admin permission
                if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.System.Configuration))
                    throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
            }
            else
            {
                // user can only save their own configuration docs
                if (user != Thread.CurrentPrincipal.Identity.Name)
                    throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
            }
        }

        private ConfigurationDocument NewDocument(ConfigurationDocumentKey key)
        {
            return new ConfigurationDocument(key.DocumentName,
                VersionUtils.ToPaddedVersionString(key.Version, false, false),
                StringUtilities.NullIfEmpty(key.User),
                StringUtilities.NullIfEmpty(key.InstanceKey));
        }

        private ConfigurationDocumentSearchCriteria BuildCurrentVersionCriteria(ConfigurationDocumentKey key)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(key);
            criteria.DocumentVersionString.EqualTo(VersionUtils.ToPaddedVersionString(key.Version, false, false));
            return criteria;
        }

		private ConfigurationDocumentSearchCriteria BuildCurrentAndPerviousVersionsCriteria(ConfigurationDocumentKey key)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(key);
            criteria.DocumentVersionString.LessThanOrEqualTo(VersionUtils.ToPaddedVersionString(key.Version, false, false));
            return criteria;
        }

		private ConfigurationDocumentSearchCriteria BuildUnversionedCriteria(ConfigurationDocumentKey key)
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


    }
}
