#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Provides a data-contract for sharing information about a settings group.
    /// </summary>
    [DataContract]
    public class SettingsGroupInfo : DataContractBase
    {
        public SettingsGroupInfo(SettingsGroupDescriptor desc)
        {
            this.Name = desc.Name;
            this.Description = desc.Description;
            this.Version = desc.Version;
            this.AssemblyQualifiedTypeName = desc.AssemblyQualifiedTypeName;
            this.HasUserScopedSettings = desc.HasUserScopedSettings;
        }

        public SettingsGroupDescriptor ToDescriptor()
        {
            return new SettingsGroupDescriptor(this.Name, this.Version, this.Description, this.AssemblyQualifiedTypeName,
                this.HasUserScopedSettings);
        }

        /// <summary>
        /// Name of the settings group.
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// Description of the settings group.
        /// </summary>
        [DataMember]
        public string Description;

        /// <summary>
        /// Settings group version.
        /// </summary>
        [DataMember]
        public Version Version;

        /// <summary>
        /// Assembly qualified type name of the class that implements the settings group.
        /// </summary>
        [DataMember]
        public string AssemblyQualifiedTypeName;

        /// <summary>
        /// Indicates whether this group has user-scoped settings.
        /// </summary>
        [DataMember]
        public bool HasUserScopedSettings;
    }

    /// <summary>
    /// Provides a data-contract for sharing information about a settings property.
    /// </summary>
    [DataContract]
    public class SettingsPropertyInfo : DataContractBase
    {
        public SettingsPropertyInfo(SettingsPropertyDescriptor desc)
        {
            this.Name = desc.Name;
            this.Description = desc.Description;
            this.TypeName = desc.TypeName;
            this.Scope = desc.Scope;
            this.DefaultValue = desc.DefaultValue;
        }

        public SettingsPropertyDescriptor ToDescriptor()
        {
            return new SettingsPropertyDescriptor(this.Name, this.TypeName, this.Description, this.Scope, this.DefaultValue);
        }

        /// <summary>
        /// Name of the property.
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// Description of the property.
        /// </summary>
        [DataMember]
        public string Description;

        /// <summary>
        /// Name of the type of the property.
        /// </summary>
        [DataMember]
        public string TypeName;

        /// <summary>
        /// String describing the scope of the property.
        /// </summary>
        [DataMember]
        public SettingScope Scope;

        /// <summary>
        /// Serialized default value of the property.
        /// </summary>
        [DataMember]
        public string DefaultValue;
    }

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
        /// <returns></returns>
        [OperationContract]
        List<SettingsGroupInfo> ListSettingsGroups();

        /// <summary>
        /// Lists the settings properties for the specified settings group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        [OperationContract]
        List<SettingsPropertyInfo> ListSettingsProperties(SettingsGroupInfo group);

        /// <summary>
        /// Gets the document specified by the name, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
        string GetConfigurationDocument(string documentName, Version version, string user, string instanceKey);

        /// <summary>
        /// Sets the content for the specified document, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
        void SetConfigurationDocument(string documentName, Version version, string user, string instanceKey, string documentContent);

        /// <summary>
        /// Removes any stored settings values for the specified group, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
        void RemoveConfigurationDocument(string documentName, Version version, string user, string instanceKey);

    }
}
