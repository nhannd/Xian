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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Configuration
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IApplicationConfigurationReadService))]
    public class ApplicationConfigurationReadService : ConfigurationServiceBase, IApplicationConfigurationReadService
    {
        #region IApplicationConfigurationReadService Members

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

        #endregion
    }
}
