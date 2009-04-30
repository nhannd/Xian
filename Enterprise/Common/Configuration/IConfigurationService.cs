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
using System.ServiceModel;
using System.Runtime.Serialization;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Configuration
{
    /// <summary>
    /// Defines a service for saving/retrieving configuration data to/from a persistent store.
    /// </summary>
    [EnterpriseCoreService]
    [ServiceContract]
    public interface IConfigurationService : ICoreServiceLayer
    {
        /// <summary>
        /// Lists settings groups installed in the local plugin base.
        /// </summary>
        [OperationContract]
		ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request);

        /// <summary>
        /// Lists the settings properties for the specified settings group.
        /// </summary>
        [OperationContract]
		ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request);

        /// <summary>
        /// Gets the document specified by the name, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
		GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request);

        /// <summary>
        /// Sets the content for the specified document, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
		SetConfigurationDocumentResponse SetConfigurationDocument(SetConfigurationDocumentRequest request);

        /// <summary>
        /// Removes any stored settings values for the specified group, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
		RemoveConfigurationDocumentResponse RemoveConfigurationDocument(RemoveConfigurationDocumentRequest request);

    }
}
