using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Describes a settings property.  A settings property is a single property belonging to a settings group.
    /// </summary>
    public class SettingsPropertyDescriptor
    {
        public static List<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName);

            return CollectionUtils.Map<PropertyInfo, SettingsPropertyDescriptor, List<SettingsPropertyDescriptor>>(
                SettingsClassMetaDataReader.GetSettingsProperties(settingsClass),
                delegate(PropertyInfo p)
                {
                    SettingsPropertyDescriptor info = new SettingsPropertyDescriptor(
                        SettingsClassMetaDataReader.GetName(p),
                        SettingsClassMetaDataReader.GetType(p).FullName,
                        SettingsClassMetaDataReader.GetDescription(p),
                        SettingsClassMetaDataReader.GetScope(p),
                        SettingsClassMetaDataReader.GetDefaultValue(p));
                    return info;
                });
        }


        private string _name;
        private string _typeName;
        private string _description;
        private SettingScope _scope;
        private string _defaultValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <param name="description"></param>
        /// <param name="scope"></param>
        /// <param name="defaultValue"></param>
        public SettingsPropertyDescriptor(string name, string typeName, string description, SettingScope scope, string defaultValue)
        {
            _name = name;
            _typeName = typeName;
            _description = description;
            _scope = scope;
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the name of the type of the property.
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
        }

        /// <summary>
        /// Gets the description of the property.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the scope of the property.
        /// </summary>
        public SettingScope Scope
        {
            get { return _scope; }
        }

        /// <summary>
        /// Gets the serialized default value of the property.
        /// </summary>
        public string DefaultValue
        {
            get { return _defaultValue; }
        }
    }
}
