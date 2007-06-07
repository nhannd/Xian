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
        }

        public SettingsGroupDescriptor ToDescriptor()
        {
            return new SettingsGroupDescriptor(this.Name, this.Version, this.Description, this.AssemblyQualifiedTypeName);
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
    [ServiceContract]
    public interface IConfigurationService : ICoreServiceLayer
    {
        /// <summary>
        /// Lists all available settings groups.
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
        /// Loads the values for the settings group specified by the name, version, user and instance key,
        /// returning the serialized values in a dictionary. The user and instance key may be null.
        /// </summary>
        [OperationContract]
        Dictionary<string, string> LoadSettingsValues(string group, Version version, string user, string instanceKey);

        /// <summary>
        /// Stores the settings values for the specified group, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
        void SaveSettingsValues(string group, Version version, string user, string instanceKey, Dictionary<string, string> values);

        /// <summary>
        /// Removes any stored settings values for the specified group, version, user and instance key.
        /// </summary>
        [OperationContract]
        void RemoveSettingsValues(string name, Version version, string user, string instanceKey);

    }
}
