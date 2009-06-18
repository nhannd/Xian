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
    public class ConfigurationService : ConfigurationServiceBase, IConfigurationService
    {
        #region IConfigurationService Members

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetSettingsMetadataCachingDirective")]
        public ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request)
        {
            IConfigurationSettingsGroupBroker broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
            return new ListSettingsGroupsResponse(
                CollectionUtils.Map<ConfigurationSettingsGroup, SettingsGroupDescriptor>(
                    broker.FindAll(),
                    delegate(ConfigurationSettingsGroup g)
                    {
                        return g.GetDescriptor();
                    }));
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetSettingsMetadataCachingDirective")]
        public ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Group, "Group");

            ConfigurationSettingsGroupSearchCriteria where =
                ConfigurationSettingsGroup.GetCriteria(request.Group);

            IConfigurationSettingsGroupBroker broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
            ConfigurationSettingsGroup group = broker.FindOne(where);

            return new ListSettingsPropertiesResponse(
                CollectionUtils.Map<ConfigurationSettingsProperty, SettingsPropertyDescriptor>(
                    group.SettingsProperties,
                    delegate(ConfigurationSettingsProperty p)
                    {
                        return p.GetDescriptor();
                    }));
        }

        [UpdateOperation]
        public ImportSettingsGroupResponse ImportSettingsGroup(ImportSettingsGroupRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.Group, "Group");

            IConfigurationSettingsGroupBroker broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
            ConfigurationSettingsGroupSearchCriteria where = ConfigurationSettingsGroup.GetCriteria(request.Group);
            ConfigurationSettingsGroup group = CollectionUtils.FirstElement(broker.Find(where));
            if (group == null)
            {
                // group doesn't exist, need to create it
                group = new ConfigurationSettingsGroup();
                group.UpdateFromDescriptor(request.Group);
                PersistenceContext.Lock(group, DirtyState.New);
            }
            else
            {
                // update group from descriptor
                group.UpdateFromDescriptor(request.Group);
            }

            if (request.Properties != null)
            {
                // update properties
                group.SettingsProperties.Clear();
                foreach (SettingsPropertyDescriptor descriptor in request.Properties)
                {
                    ConfigurationSettingsProperty property = new ConfigurationSettingsProperty();
                    property.UpdateFromDescriptor(descriptor);
                    group.SettingsProperties.Add(property);
                }
            }

            PersistenceContext.SynchState();

            return new ImportSettingsGroupResponse();
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetDocumentCachingDirective")]
        public GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DocumentKey, "DocumentKey");
			
            return GetConfigurationDocumentHelper(request.DocumentKey);
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [Audit(typeof(ConfigurationServiceRecorder))]
		public SetConfigurationDocumentResponse SetConfigurationDocument(SetConfigurationDocumentRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DocumentKey, "DocumentKey");

			CheckWriteAccess(request.DocumentKey);

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

			CheckWriteAccess(request.DocumentKey);
   
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


        private ConfigurationDocument NewDocument(ConfigurationDocumentKey key)
        {
            return new ConfigurationDocument(key.DocumentName,
                VersionUtils.ToPaddedVersionString(key.Version, false, false),
                StringUtilities.NullIfEmpty(key.User),
                StringUtilities.NullIfEmpty(key.InstanceKey));
        }

		private ConfigurationDocumentSearchCriteria BuildCurrentAndPerviousVersionsCriteria(ConfigurationDocumentKey key)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(key);
            criteria.DocumentVersionString.LessThanOrEqualTo(VersionUtils.ToPaddedVersionString(key.Version, false, false));
            return criteria;
        }


    }
}
